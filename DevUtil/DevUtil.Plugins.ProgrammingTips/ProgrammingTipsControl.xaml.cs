/*
 * SharpDevelop으로 작성되었습니다.
 * 사용자: RYZEN3200G
 * 날짜: 2025-10-23
 * 시간: 오후 2:53
 * 
 * 이 템플리트를 변경하려면 [도구->옵션->코드 작성->표준 헤더 편집]을 이용하십시오.
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DevUtil.Core;

namespace DevUtil.Plugins.ProgrammingTips
{
	/// <summary>
	/// Interaction logic for ProgrammingTipsControl.xaml
	/// </summary>
	public partial class ProgrammingTipsControl : UserControl, IPluginUserControl
	{
        public string PluginName { get { return "프로그래밍팁"; } }
        public string Description { get { return "유용한 프로그래밍 팁과 코드 예제"; } }
		public string PluginTitle { get; set; }
		
		public ProgrammingTipsControl()
		{
			InitializeComponent();
		}
        public UserControl GetControl()
        {
        	return new ProgrammingTipsControl();
        }

        public void Initialize()
        {
            // 플러그인 초기화 로직
            UpdateTitle();
        }

        private void UpdateTitle()
	    {
	        Version version = Assembly.GetExecutingAssembly().GetName().Version;
	        DateTime buildTime = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
	        
	        PluginTitle = string.Format("{0} - [ Build : {1:yyyy-MM-dd HH:mm:ss} - Ver : {2} ]",
	            PluginName, buildTime, version);
	    }     
        
        public void Cleanup()
        {
            // 플러그인 종료 시 정리 로직
        }		
	}
}