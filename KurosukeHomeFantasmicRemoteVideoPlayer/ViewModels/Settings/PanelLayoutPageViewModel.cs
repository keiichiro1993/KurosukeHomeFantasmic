using CommonUtils;
using KurosukeHomeFantasmicRemoteVideoPlayer.Models;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.AtomPub;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.ViewModels.Settings
{
    internal class PanelLayoutPageViewModel : ViewModelBase, IDisposable
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

        public void Init()
        {
            LEDPanelUnitSets.CollectionChanged += LEDPanelUnitSets_CollectionChanged;
        }

        private LEDPanelUnitSet previouslyRemovedItem = null;
        private async void LEDPanelUnitSets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // this is the only way to detect reorder as ListView calls Remove/Add when reordering the item.
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                previouslyRemovedItem = e.OldItems[0] as LEDPanelUnitSet;
                return;
            }

            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                previouslyRemovedItem = null;
                return;
            }
            else if (previouslyRemovedItem?.SerialDeviceId == (e.NewItems[0] as LEDPanelUnitSet)?.SerialDeviceId)
            {
                // this should mean reordering

                IsLoading = true;
                // reset y coordinate
                for (var i = 0; i < LEDPanelUnitSets.Count; i++)
                {
                    LEDPanelUnitSets[i].Coordinate.Y = i;
                }
                try
                {
                    var ledUnitSetsHelper = new Utils.DBHelpers.PanelLayoutHelper();
                    await ledUnitSetsHelper.SaveLEDPanelUnitSets(LEDPanelUnitSets.ToList());
                }
                catch (Exception ex)
                {
                    await DebugHelper.ShowErrorDialog(ex, "Failed to save reordered combined controls.");
                }
                IsLoading = false;
            }
        }

        public void Dispose()
        {
            LEDPanelUnitSets.CollectionChanged -= LEDPanelUnitSets_CollectionChanged;
        }
    }
}
