/*
 * SharpDevelop으로 작성되었습니다.
 * 사용자: RYZEN3200G
 * 날짜: 2025-10-23
 * 시간: 오후 1:19
 * 
 * 이 템플리트를 변경하려면 [도구->옵션->코드 작성->표준 헤더 편집]을 이용하십시오.
 */
 
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DevUtil.Core;

namespace DevUtil.Plugins.CodeManager
{
	/// <summary>
	/// Interaction logic for CodeManagerControl.xaml
	/// </summary>
    public partial class CodeManagerControl : UserControl, IPluginUserControl
    {
        public string PluginName { get { return "코드관리"; } }
        public string Description { get { return "코드 스니펫 및 템플릿 관리"; } }
        public string PluginTitle { get; set; }

        public CodeManagerControl()
        {
            InitializeComponent();
        }
        public UserControl GetControl()
        {
        	return new CodeManagerControl();
        }

        public void Initialize()
        {
        	
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
        }
    }
}