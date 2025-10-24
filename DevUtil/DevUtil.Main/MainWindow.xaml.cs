/*
 * SharpDevelop으로 작성되었습니다.
 * 사용자: RYZEN3200G
 * 날짜: 2025-10-23
 * 시간: 오후 12:41
 * 
 * 이 템플리트를 변경하려면 [도구->옵션->코드 작성->표준 헤더 편집]을 이용하십시오.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using DevUtil.Core;

namespace DevUtil.Main
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
    public partial class MainWindow : Window
    {
        // 플러그인 타입만 저장 (인스턴스 아님)
        private Dictionary<string, Type> pluginTypes;
        private Dictionary<string, TabItem> openTabs;
        private DispatcherTimer statusTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeControl();
            pluginTypes = new Dictionary<string, Type>();
            openTabs = new Dictionary<string, TabItem>();
            
            InitializeStatusBar();
            LoadPlugins();
            LoadDynamicMenu();
        }
        
        
        //참고 : https://madbrain.tistory.com/entry/Assembly-%EB%B2%84%EC%A0%84%EC%9D%84-%EC%9D%B4%EC%9A%A9%ED%95%9C-%EB%B9%8C%EB%93%9C-%EB%82%A0%EC%A7%9C-%EB%B0%8F-%EB%B2%84%EC%A0%84-%ED%91%9C%EC%8B%9C
        private void InitializeControl()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            string strVersion = string.Format("[ Last Build : {0} - Ver : {1} ]",
                GetBuildTime(version), version);

            this.Title = "DevUtil - 개발 도구 모음 - " + strVersion;
        }

        public DateTime GetBuildTime(Version version)
        {
		    string filePath = Assembly.GetExecutingAssembly().Location;
		    return File.GetLastWriteTime(filePath);
        }        


        /// <summary>
        /// StatusBar 초기화
        /// </summary>
        private void InitializeStatusBar()
        {
            // 상태 메시지가 자동으로 사라지도록 타이머 설정
            statusTimer = new DispatcherTimer();
            statusTimer.Interval = TimeSpan.FromSeconds(5);
            statusTimer.Tick += (s, e) =>
            {
                UpdateStatus("준비", Brushes.Green);
                statusTimer.Stop();
            };
        }

        /// <summary>
        /// StatusBar 업데이트 (StatusTextBlock이 XAML에 정의되어 있어야 함)
        /// </summary>
        private void UpdateStatus(string message, Brush color = null)
        {
            if (StatusTextBlock != null)
            {
                StatusTextBlock.Text = message;
                StatusTextBlock.Foreground = color ?? Brushes.White;
                
                // 5초 후 기본 상태로 복귀
                statusTimer.Stop();
                statusTimer.Start();
            }
        }
        
        
        private void LoadDynamicMenu()
        {
        	
				/* //만약 DB를 사용한다면, 다음과 같은 테이블을 가정할 수 있습니다.
				CREATE TABLE Menu (
				    Id INTEGER PRIMARY KEY AUTOINCREMENT,
				    GroupName TEXT,
				    MenuName TEXT,
				    PluginKey TEXT
				); 
				 */         	
				
				/* // 그리고 C#에서 다음과 같이 불러올 수 있습니다.
				using (var conn = new SQLiteConnection("Data Source=menu.db"))
				{
				    conn.Open();
				    var cmd = new SQLiteCommand("SELECT GroupName, MenuName, PluginKey FROM Menu", conn);
				    var reader = cmd.ExecuteReader();
				
				    var groups = new Dictionary<string, MenuGroup>();
				
				    while (reader.Read())
				    {
				        string groupName = reader["GroupName"].ToString();
				        string menuName = reader["MenuName"].ToString();
				        string tag = reader["PluginKey"].ToString();
				
				        if (!groups.ContainsKey(groupName))
				            groups[groupName] = new MenuGroup { Name = groupName };
				
				        groups[groupName].Items.Add(new MenuItemInfo { Name = menuName, Tag = tag });
				    }
				
				    LoadMenu(groups.Values.ToList());
				}
				
				 */ 
        	
            // 실제로는 DB에서 읽어올 수 있음 (예시 데이터)
            var menuGroups = new List<MenuGroup>
            {
                new MenuGroup
                {
                    Name = "관리자",
                    Items = new List<MenuItemInfo>
                    {
                        new MenuItemInfo { Name = "코드관리", Tag = "CodeManager" },
                        new MenuItemInfo { Name = "프로젝트관리", Tag = "ProjectManager" }
                    }
                },
                new MenuGroup
                {
                    Name = "업무일지",
                    Items = new List<MenuItemInfo>
                    {
                        new MenuItemInfo { Name = "업무일지", Tag = "WorkLog" }
                    }
                },
                new MenuGroup
                {
                    Name = "프로그래밍팁",
                    Items = new List<MenuItemInfo>
                    {
                        new MenuItemInfo { Name = "프로그래밍팁", Tag = "ProgrammingTips" }
                    }
                }
            };

            // TreeView 구성
            foreach (var group in menuGroups)
            {
                var groupItem = new TreeViewItem
                {
                    Header = group.Name,
                    Foreground = System.Windows.Media.Brushes.White,
                    IsExpanded = true
                };

                foreach (var item in group.Items)
                {
                    var menuItem = new TreeViewItem
                    {
                        Header = item.Name,
                        Tag = item.Tag,
                        Foreground = System.Windows.Media.Brushes.White
                    };
                    menuItem.PreviewMouseLeftButtonUp += TreeViewItem_MouseLeftButtonUp;
                    groupItem.Items.Add(menuItem);
                }

                MenuTreeView.Items.Add(groupItem);
            }
        }
        
        
        /// <summary>
        /// Plugins 폴더에서 DLL을 로드하고 IPluginUserControl 구현 클래스를 등록
        /// </summary>
        private void LoadPlugins()
        {
            try
            {
                string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

                if (!Directory.Exists(pluginPath))
                {
                    Directory.CreateDirectory(pluginPath);
                    UpdateStatus("Plugins 폴더가 생성되었습니다. 플러그인 DLL을 넣어주세요.", Brushes.Orange);
                    return;
                }

                string[] dllFiles = Directory.GetFiles(pluginPath, "DevUtil.Plugins.*.dll");
                if (dllFiles.Length == 0)
                {
                    UpdateStatus("플러그인 DLL 파일이 없습니다. Plugins 폴더를 확인하세요.", Brushes.Orange);
                    return;
                }

                int successCount = 0;
                int failCount = 0;
                List<string> loadedPlugins = new List<string>();

                foreach (string dllFile in dllFiles)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(dllFile);
                        Type[] types = assembly.GetTypes();
                        bool foundPlugin = false;

                        foreach (Type type in types)
                        {
                            if (typeof(IPluginUserControl).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                            {
                                string pluginKey = GetPluginKey(Path.GetFileNameWithoutExtension(dllFile));
                                pluginTypes[pluginKey] = type;
                                foundPlugin = true;
                                successCount++;
                                loadedPlugins.Add(pluginKey);
                            }
                        }

                        if (!foundPlugin)
                        {
                            failCount++;
                            System.Diagnostics.Debug.WriteLine(
                                string.Format("경고: IPluginUserControl 구현 클래스를 찾을 수 없습니다 - {0}", 
                                Path.GetFileName(dllFile)));
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        System.Diagnostics.Debug.WriteLine(
                            string.Format("플러그인 로드 실패: {0} - {1}", 
                            Path.GetFileName(dllFile), ex.Message));
                    }
                }

                // 로드 결과를 StatusBar에 표시
                if (successCount > 0)
                {
                    string pluginList = string.Join(", ", loadedPlugins);
                    UpdateStatus(
                        string.Format("플러그인 {0}개 로드 완료: {1}", successCount, pluginList), 
                        Brushes.LightGreen);
                }
                else
                {
                    UpdateStatus("로드된 플러그인이 없습니다.", Brushes.Orange);
                }

                // 실패한 플러그인이 있으면 디버그 출력
                if (failCount > 0)
                {
                    System.Diagnostics.Debug.WriteLine(
                        string.Format("로드 실패한 플러그인: {0}개", failCount));
                }
            }
            catch (Exception ex)
            {
                UpdateStatus(string.Format("플러그인 로드 중 오류: {0}", ex.Message), Brushes.Red);
                System.Diagnostics.Debug.WriteLine(string.Format("플러그인 로드 오류: {0}", ex.ToString()));
            }
        }
        

        /// <summary>
        /// DLL 파일명에서 플러그인 키 추출
        /// </summary>
        private string GetPluginKey(string dllName)
        {
            if (dllName.StartsWith("DevUtil.Plugins."))
                return dllName.Substring("DevUtil.Plugins.".Length);
            return dllName;
        }
        
		private void TreeViewItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
		    var item = sender as TreeViewItem;
		    if (item != null && item.Tag != null)
		    {
		        string pluginKey = item.Tag.ToString();
		        OpenPluginTab(pluginKey);
		        e.Handled = true; // 이벤트 중복 방지
		    }
		}        

        /// <summary>
        /// TreeView 아이템 선택 이벤트
        /// </summary>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView treeView = sender as TreeView;
            TreeViewItem selectedItem = treeView.SelectedItem as TreeViewItem;

            if (selectedItem != null && selectedItem.Tag != null)
            {
                string pluginKey = selectedItem.Tag.ToString();
                OpenPluginTab(pluginKey);
            }
        }

        /// <summary>
        /// 플러그인 탭 열기 (매번 새 인스턴스 생성)
        /// </summary>
        private void OpenPluginTab(string pluginKey)
        {
        	
            // 이미 열린 탭이 있으면 활성화
            if (openTabs.ContainsKey(pluginKey))
            {
            	var existingTab = openTabs[pluginKey];
            	var info = existingTab.Tag as PluginTagInfo;
            	
                MainTabControl.SelectedItem = openTabs[pluginKey];
                UpdateStatus(string.Format("'{0}' 탭으로 이동", info.Title), Brushes.LightBlue); //pluginKey
                return;
            }

            if (!pluginTypes.ContainsKey(pluginKey))
            {
                UpdateStatus(string.Format("플러그인을 찾을 수 없습니다: {0}", pluginKey), Brushes.Red);
                return;
            }

            try
            {
                // 새로운 플러그인 인스턴스 생성
                Type type = pluginTypes[pluginKey];
                IPluginUserControl plugin = (IPluginUserControl)Activator.CreateInstance(type);
                
                plugin.Initialize();


                UserControl control = plugin.GetControl();

                TabItem tabItem = new TabItem
                {
                    Header = plugin.PluginName,
                    //Header = plugin.PluginTitle, // 2025.10.24 주석처리 원래대로 변경함.
                    Content = control,
				    Tag = new PluginTagInfo
				    {
				        Key = pluginKey,
				        Title = plugin.PluginTitle
				    }
                };
                var info = tabItem.Tag as PluginTagInfo;

                MainTabControl.Items.Add(tabItem);
                openTabs[pluginKey] = tabItem;
                MainTabControl.SelectedItem = tabItem;
                
                UpdateStatus(string.Format("'{0}' 플러그인 로드 완료", info.Title), Brushes.LightGreen);
            }
            catch (Exception ex)
            {
                UpdateStatus(
                    string.Format("플러그인 탭 열기 실패: {0}", pluginKey), 
                    Brushes.Red);
                System.Diagnostics.Debug.WriteLine(
                    string.Format("플러그인 탭 열기 실패: {0}\n{1}", pluginKey, ex.ToString()));
            }
        }
        
        /// <summary>
        /// 탭 닫기 버튼 클릭
        /// </summary>
        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TabItem tabItem = button.Tag as TabItem;

            if (tabItem != null && tabItem != StartPageTab)
            {
                string pluginKey = tabItem.Tag as string;
                if (pluginKey != null && openTabs.ContainsKey(pluginKey))
                {
                    openTabs.Remove(pluginKey);
                    UpdateStatus(string.Format("'{0}' 탭 닫기", pluginKey), Brushes.Gray);
                }

                MainTabControl.Items.Remove(tabItem);
            }
        }


        /// <summary>
        /// 윈도우 종료 시 열린 탭 정리
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            foreach (var tab in openTabs.Values.ToList())
            {
                MainTabControl.Items.Remove(tab);
            }
            openTabs.Clear();

            if (statusTimer != null)
            {
                statusTimer.Stop();
            }

            base.OnClosed(e);
        }
    }
    
	public class PluginTagInfo
	{
	    public string Key { get; set; }
	    public string Title { get; set; }
	}    
    
    
    // 메뉴용 데이터 클래스
    public class MenuGroup
    {
        public string Name { get; set; }
        public List<MenuItemInfo> Items { get; set; }

        public MenuGroup()
        {
            Items = new List<MenuItemInfo>();
        }
    }

    public class MenuItemInfo
    {
        public string Name { get; set; }
        public string Tag { get; set; }
    }    
}