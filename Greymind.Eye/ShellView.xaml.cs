using System;

namespace Greymind.Eye
{
    public partial class ShellView : IShellView
    {
        public void BringFocusToWindow()
        {
            this.Dispatcher.BeginInvoke(new Func<bool>(Activate));
        }
    }
}