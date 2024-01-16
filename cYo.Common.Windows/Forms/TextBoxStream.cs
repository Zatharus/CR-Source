// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TextBoxStream
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using cYo.Common.Threading;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TextBoxStream : Stream
  {
    private TextBoxBase textBox;

    public TextBoxStream(TextBoxBase textBox)
    {
      this.textBox = textBox;
      this.textBox.Disposed += new EventHandler(this.textBox_Disposed);
    }

    private void textBox_Disposed(object sender, EventArgs e) => this.textBox = (TextBoxBase) null;

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override void Flush()
    {
    }

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.textBox == null || this.textBox.InvokeIfRequired((Action) (() => this.Write(buffer, offset, count))))
        return;
      using (ItemMonitor.Lock((object) this))
      {
        this.textBox.AppendText(Encoding.Default.GetString(buffer, offset, count));
        this.textBox.SelectionStart = this.textBox.Text.Length;
        this.textBox.ScrollToCaret();
      }
    }
  }
}
