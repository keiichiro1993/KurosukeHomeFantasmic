using System;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Utils
{
    public class ItemDeleteButtonClickedEventArgs<T> : EventArgs
    {
        private T item;

        public ItemDeleteButtonClickedEventArgs(T item)
        {
            this.item = item;
        }

        public T DeleteItem
        {
            get { return item; }
        }
    }
}
