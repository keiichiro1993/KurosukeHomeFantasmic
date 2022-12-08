using CommonUtils;
using KurosukeBonjourService;
using KurosukeBonjourService.Models.BonjourEventArgs;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils;
using KurosukeHueClient.Utils;
using System;
using System.Linq;
using Windows.Media.Playback;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Players
{
    internal sealed partial class RemoteVideoTimelinePlayerChild : TimelinePlayerBase
    {
        public RemoteVideoTimelinePlayerChild()
        {
            this.InitializeComponent();
        }

        private TimelineRemoteVideoItem videoItem;
        private BonjourClient client;

        public override async void UpdatePlaybackState()
        {
            // Skip if null
            if (PlaybackState == null || Timeline == null)
            {
                return;
            }

            if (PlaybackState == MediaPlaybackState.Paused)
            {
                if (client == null)
                {
                    return;
                }

                lock (client)
                {
                    if (client.Status == ConnectionStatus.Connected)
                    {
                        var videoPosition = CurrentPosition - videoItem?.StartTime;
                        client.SendMessage(new PlayVideoEventArgs
                        {
                            VideoStatus = KurosukeBonjourService.Models.WebSocketServices.PlayVideoService.PlayVideoServiceStatus.Pause,
                            VideoName = videoItem?.RemoteVideoAsset.Info.Name,
                            VideoTime = videoPosition ?? TimeSpan.Zero
                        });
                        client.Disconnect();
                    }
                }
            }
            else if (PlaybackState == MediaPlaybackState.Playing)
            {
                var needInit = false;
                lock (client)
                {
                    if (client == null)
                    {
                        //TODO: さがして、無かったら作ってAppGlobalVariablesに突っ込む関数にする
                        client = (from bonjourClient in AppGlobalVariables.BonjourClients
                                  where bonjourClient.Device.DomainName == Timeline.TargetDomainName
                                  select bonjourClient).FirstOrDefault();
                    }

                    if (client.Status == ConnectionStatus.Disconnected)
                    {
                        client.Status = ConnectionStatus.Connecting;
                        needInit = true;
                    }
                }

                if (needInit)
                {
                    await client.Connect();
                }
            }
        }

        public override async void UpdatePosition()
        {
            // Skip if null or disconnected
            if (Timeline == null ||
                CurrentPosition == null ||
                client == null ||
                client.Status != ConnectionStatus.Connected)
            {
                return;
            }

            var candidates = from TimelineRemoteVideoItem item in Timeline.TimelineItems
                             where item.StartTime <= CurrentPosition && item.EndTime >= CurrentPosition
                             orderby item.StartTime descending // choose the item with latest StartTime
                             select item;
            if (!candidates.Any())
            {
                videoItem = null;
                return;
            }

            var targetItem = candidates.First();
            if (videoItem == null || videoItem.ItemId != targetItem.ItemId)
            {
                DebugHelper.WriteDebugLog($"Remote Video: Playing '{videoItem.RemoteVideoAsset.Info.Name}({videoItem.ItemId})' on HOST '{videoItem.RemoteVideoAsset.DomainName}'");
                videoItem = targetItem;
            }

            try
            {
                var videoPosition = CurrentPosition - videoItem.StartTime;
                client.SendMessage(new PlayVideoEventArgs
                {
                    VideoName = videoItem.RemoteVideoAsset.Info.Name,
                    VideoStatus = KurosukeBonjourService.Models.WebSocketServices.PlayVideoService.PlayVideoServiceStatus.Play,
                    VideoTime = videoPosition
                });
            }
            catch (OperationCanceledException)
            {
                //TODO: await 無し非同期なのでタスクそれぞれ確認しないと catch 不能なんだが？
                // https://qiita.com/tera1707/items/d5a3bc12ffa5f80069a1
                // エラー出力コンソールみたいの実装してそこに流す案に一票
            }
            catch (Exception ex)
            {
                OnMemoryCache.GlobalViewModel.GlobalPlaybackState = MediaPlaybackState.Paused;
                await DebugHelper.ShowErrorDialog(ex, "Error in Hue Player");
            }
        }
    }
}
