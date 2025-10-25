/*
 * SharpDevelop으로 작성되었습니다.
 * 사용자: sigmak
 * 날짜: 2025-10-25
 * 시간: 오후 11:45
 * 
 * 이 템플리트를 변경하려면 [도구->옵션->코드 작성->표준 헤더 편집]을 이용하십시오.
 */
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;


namespace DevUtil.Main
{
	/// <summary>
	/// Interaction logic for SignUpWindow.xaml
	/// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
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

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            // 입력 값 가져오기
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string name = NameTextBox.Text.Trim();

            // 유효성 검사
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("아이디를 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (username.Length < 4)
            {
                MessageBox.Show("아이디는 4자 이상이어야 합니다.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("이메일을 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("올바른 이메일 형식이 아닙니다.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("비밀번호를 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 8)
            {
                MessageBox.Show("비밀번호는 8자 이상이어야 합니다.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("이름을 입력해주세요.", "입력 오류", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TermsCheckBox.IsChecked.GetValueOrDefault())
            {
                MessageBox.Show("이용약관 및 개인정보처리방침에 동의해주세요.", "약관 동의 필요", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: 실제 회원가입 로직 구현
            MessageBox.Show("회원가입이 완료되었습니다!", "회원가입 완료", 
                MessageBoxButton.OK, MessageBoxImage.Information);

            // 로그인 화면으로 이동
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Login_Click(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Terms_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("이용약관 내용을 표시합니다.", "이용약관", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Privacy_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("개인정보처리방침 내용을 표시합니다.", "개인정보처리방침", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}