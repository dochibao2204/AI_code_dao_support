using CopilotMockup.ViewModels;
using System.Windows;

namespace CopilotMockup.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        // private async void OnSendClicked(object sender, RoutedEventArgs e) bỏ đi không chơi ngầm trực tiếp mà dùng MVVM
        // {
        //     string prompt = PromptInput.Text.Trim();
        //     if (string.IsNullOrWhiteSpace(prompt)) return;

        //     PromptInput.Clear();
        //     await _viewModel.SendMessageAsync(prompt);
        // }

        // private void ToggleHistoryPanel(object sender, RoutedEventArgs e)
        // {
        //     HistoryPanel.Visibility = HistoryPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        //     HistoryColumn.Width = HistoryPanel.Visibility == Visibility.Visible ? new GridLength(250) : new GridLength(0);
        // }

        private void ToggleHistoryPanel(object sender, RoutedEventArgs e)
        {
            if (HistoryPanel.Visibility == Visibility.Visible)
            {
                HistoryPanel.Visibility = Visibility.Collapsed;
                HistoryColumn.Width = new GridLength(0);
            }
            else
            {
                HistoryPanel.Visibility = Visibility.Visible;
                HistoryColumn.Width = new GridLength(250);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        // private void PromptInput_GotFocus(object sender, RoutedEventArgs e) bỏ đi không chơi ngầm trực tiếp mà dùng MVVM
        // {
        //     if (PromptInput.Text == "Nhập câu hỏi hoặc prompt của bạn tại đây...")
        //     {
        //         PromptInput.Text = "";
        //         PromptInput.Foreground = System.Windows.Media.Brushes.Black;
        //     }
        // }

        // private void PromptInput_LostFocus(object sender, RoutedEventArgs e) bỏ đi không chơi ngầm trực tiếp mà dùng MVVM
        // {
        //     if (string.IsNullOrWhiteSpace(PromptInput.Text))
        //     {
        //         PromptInput.Text = "Nhập câu hỏi hoặc prompt của bạn tại đây...";
        //         PromptInput.Foreground = System.Windows.Media.Brushes.Gray;
        //     }
        // }
    }
}