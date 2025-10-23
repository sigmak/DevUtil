/*
 * SharpDevelop으로 작성되었습니다.
 * 사용자: RYZEN3200G
 * 날짜: 2025-10-23
 * 시간: 오후 1:22
 * 
 * 이 템플리트를 변경하려면 [도구->옵션->코드 작성->표준 헤더 편집]을 이용하십시오.
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DevUtil.Core;

namespace DevUtil.Plugins.ProjectManager
{
	/// <summary>
	/// Interaction logic for ProjectManagerControl.xaml
	/// </summary>
	public partial class ProjectManagerControl : UserControl, IPluginUserControl
	{
        public string PluginName { get { return "프로젝트관리"; } }
        public string Description { get { return "프로젝트 일정 및 진행 상황 관리"; } }
        public string PluginTitle { get; set; }
        
		public ProjectManagerControl()
		{
			InitializeComponent();
		}
		
        public UserControl GetControl()
        {
        	return new ProjectManagerControl();
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
	        
	        //PluginTitle = string.Format("{0} - [ Build : {1:yyyy-MM-dd HH:mm:ss} - Ver : {2} ]",
	        //    PluginName, buildTime, version);
   	        PluginTitle = string.Format("{0}-Ver {1}",
	            PluginName, version);	        
	    }        
        

        public void Cleanup()
        {
            // 플러그인 종료 시 정리 로직
        }		
	}
}