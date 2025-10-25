/*
 * SharpDevelop으로 작성되었습니다.
 * 사용자: sigmak
 * 날짜: 2025-10-24
 * 시간: 오후 10:28
 * 
 * 이 템플리트를 변경하려면 [도구->옵션->코드 작성->표준 헤더 편집]을 이용하십시오.
 */
using System.Windows;
using System.Windows.Input;

namespace DevUtil.Main
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // 로그인 로직 구현
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("아이디와 비밀번호를 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: 실제 인증 로직 구현
            MessageBox.Show("로그인 성공!", "알림", 
                MessageBoxButton.OK, MessageBoxImage.Information);
            
            // 메인 윈도우 열기 (필요시 주석 해제)
             MainWindow mainWindow = new MainWindow();
             mainWindow.Show();
             this.Close();
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("비밀번호 찾기 기능은 준비 중입니다.", "알림", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SignUp_Click(object sender, MouseButtonEventArgs e)
        {
            SignUpWindow signUpWindow = new SignUpWindow();
            signUpWindow.Show();
            this.Close();
        }
    }
}