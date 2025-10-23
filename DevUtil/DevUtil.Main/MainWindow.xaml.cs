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

        public MainWindow()
        {
            InitializeComponent();
            InitializeControl();
            pluginTypes = new Dictionary<string, Type>();
            openTabs = new Dictionary<string, TabItem>();
            
            LoadPlugins();
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
                    MessageBox.Show("Plugins 폴더가 생성되었습니다.\n플러그인 DLL을 넣어주세요.",
                        "정보", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string[] dllFiles = Directory.GetFiles(pluginPath, "DevUtil.Plugins.*.dll");
                if (dllFiles.Length == 0)
                {
                    MessageBox.Show(
                        string.Format("Plugins 폴더 경로: {0}\n\n'플러그인 DLL(DevUtil.Plugins.*.dll)' 파일이 없습니다.", pluginPath),
                        "플러그인 없음", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

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

                                MessageBox.Show(string.Format(
                                    "플러그인 등록 완료!\n\n파일: {0}\n타입: {1}\n키: {2}",
                                    Path.GetFileName(dllFile), type.FullName, pluginKey),
                                    "등록 성공", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }

                        if (!foundPlugin)
                        {
                            MessageBox.Show(
                                string.Format("IPluginUserControl 구현 클래스를 찾을 수 없습니다.\n{0}", Path.GetFileName(dllFile)),
                                "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            string.Format("플러그인 로드 실패: {0}\n\n{1}", Path.GetFileName(dllFile), ex.Message),
                            "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (pluginTypes.Count == 0)
                {
                    MessageBox.Show("로드된 플러그인이 없습니다.\nPlugins 폴더를 확인하세요.",
                        "정보", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("플러그인 로드 중 오류: {0}", ex.Message),
                    "오류", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MainTabControl.SelectedItem = openTabs[pluginKey];
                return;
            }

            if (!pluginTypes.ContainsKey(pluginKey))
            {
                MessageBox.Show(string.Format("플러그인을 찾을 수 없습니다: {0}", pluginKey),
                    "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    //Header = plugin.PluginName,
                    Header = plugin.PluginTitle,
                    Content = control,
                    Tag = pluginKey
                };

                MainTabControl.Items.Add(tabItem);
                openTabs[pluginKey] = tabItem;
                MainTabControl.SelectedItem = tabItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("플러그인 탭 열기 실패: {0}\n\n{1}", pluginKey, ex.Message),
                    "오류", MessageBoxButton.OK, MessageBoxImage.Error);
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

            base.OnClosed(e);
        }
    }
}