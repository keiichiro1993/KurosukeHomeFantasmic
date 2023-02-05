using CommonUtils;
using KurosukeHomeFantasmicRemoteVideoPlayer.Models;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.AtomPub;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.ViewModels.Settings
{
    internal class PanelLayoutPageViewModel : ViewModelBase
    {
        public ObservableCollection<LEDPanelUnitSet> LEDPanelUnitSets
        {
            get { return AppGlobalVariables.LEDPanelUnitSets; }
        }

        public string UnitPixelWidth
        {
            get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelWidth).ToString(); }
            set
            {
                int parsedValue;
                if (int.TryParse(value, out parsedValue))
                {
                    SettingsHelper.WriteSettings(SettingNameMappings.UnitPixelWidth.Key, parsedValue);
                }
                //TODO: turn textbox outline to red if parse failed
            }
        }

        public string UnitPixelHeight
        {
            get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelHeight).ToString(); }
            set
            {
                int parsedValue;
                if (int.TryParse(value, out parsedValue))
                {
                    SettingsHelper.WriteSettings(SettingNameMappings.UnitPixelHeight.Key, parsedValue);
                }
            }
        }

        public string UnitHorizontalPanelCount
        {
            get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitHorizontalPanelCount).ToString(); }
            set
            {
                int parsedValue;
                if (int.TryParse(value, out parsedValue))
                {
                    SettingsHelper.WriteSettings(SettingNameMappings.UnitHorizontalPanelCount.Key, parsedValue);
                }
            }
        }

        public string UnitVerticalPanelCount
        {
            get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitVerticalPanelCount).ToString(); }
            set
            {
                int parsedValue;
                if (int.TryParse(value, out parsedValue))
                {
                    SettingsHelper.WriteSettings(SettingNameMappings.UnitVerticalPanelCount.Key, parsedValue);
                }
            }
        }
    }
}
