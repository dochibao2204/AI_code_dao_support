using CopilotMockup.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CopilotMockup.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatItem> Messages { get; } = new();
        public ObservableCollection<ChatItem> FirstPromptHistory { get; } = new();

        private bool _isFirstPrompt = true;
        private readonly HttpClient _httpClient = new();
        private readonly string apiKey = "AIzaSyDjCHD_iHrNsNHMibe8MgqDgVKLP5YTlIA";

        private string _promptInputText = "Nhập câu hỏi hoặc prompt của bạn tại đây...";
        public string PromptInputText
        {
            get => _promptInputText;
            set
            {
            if (_promptInputText != value)
                {
                    _promptInputText = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsPromptNotEmpty));

                    // Cập nhật trạng thái có thể thực thi của lệnh SendCommand
                    (SendCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }
        
        public bool IsPromptNotEmpty => !string.IsNullOrWhiteSpace(PromptInputText);

        public ICommand SendCommand { get; }
        // public ICommand StartNewChatCommand { get; }
        // public ICommand StartNewProjectCommand { get; }
        // public ICommand SelectLinkFolderCommand { get; }
        // public ICommand UploadCommand { get; }
        // public ICommand TakeScreenshotCommand { get; }

        public MainViewModel()
        {
            SendCommand = new RelayCommand(async () => await SendMessageAsync(PromptInputText), () => IsPromptNotEmpty);

            // StartNewChatCommand = new RelayCommand(() => Debug.WriteLine("Start new chat"));
            // StartNewProjectCommand = new RelayCommand(() => Debug.WriteLine("Start new project"));
            // SelectLinkFolderCommand = new RelayCommand(() => Debug.WriteLine("Select folder"));
            // UploadCommand = new RelayCommand(() => Debug.WriteLine("Upload clicked"));
            // TakeScreenshotCommand = new RelayCommand(() => Debug.WriteLine("Screenshot taken"));
        }

        public async Task<string> SendMessageAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return string.Empty;

            Messages.Add(new ChatItem { IsUser = true, Text = prompt });

            if (_isFirstPrompt)
            {
                FirstPromptHistory.Add(new ChatItem { IsUser = true, Text = prompt });
                _isFirstPrompt = false;
            }

             // Delete prompt in TextBox after send
            PromptInputText = string.Empty;

            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[] {
                    new {
                        parts = new[] {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);

            string? reply = doc.RootElement
                              .GetProperty("candidates")[0]
                              .GetProperty("content")
                              .GetProperty("parts")[0]
                              .GetProperty("text")
                              .GetString();

            if (!string.IsNullOrWhiteSpace(reply))
            {
                Messages.Add(new ChatItem { IsUser = false, Text = reply });
            }

            return reply ?? string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<bool>? _canExecute;
        private readonly Func<Task> _executeAsync;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();
        // public bool CanExecute(object? parameter) => !string.IsNullOrEmpty(PromptInputText) && PromptInputText != "Nhập câu hỏi hoặc prompt của bạn tại đây...";
        public async void Execute(object? parameter) => await _executeAsync();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
