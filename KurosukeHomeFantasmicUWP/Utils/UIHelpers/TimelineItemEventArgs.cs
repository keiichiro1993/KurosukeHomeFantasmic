using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Utils.UIHelpers
{
    public class PositionChangedEventArgs : System.EventArgs
    {
        private double currentPosition;
        private double scale;

        public PositionChangedEventArgs(double currentPosition, double scale)
        {
            this.currentPosition = currentPosition;
            this.scale = scale;
        }

        public double CurrentPosition
        {
            get { return currentPosition; }
        }
        public double Scale
        {
            get { return scale; }
        }
    }

    public class ItemDeleteButtonClickedEventArgs<T> : System.EventArgs
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
    public delegate void DeleteButtonClickedEventHandler<T>(object sender, ItemDeleteButtonClickedEventArgs<T> args);
}
