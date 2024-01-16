// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.PasswordTextBox
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class PasswordTextBox : TextBox
  {
    private const string dummyPassword = "1234";
    private string password;

    public PasswordTextBox() => this.UseSystemPasswordChar = true;

    public string Password
    {
      get => this.password;
      set
      {
        if (value == this.password)
          return;
        this.Text = string.IsNullOrEmpty(value) ? string.Empty : "1234";
        this.password = value;
      }
    }

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      this.password = cYo.Common.Cryptography.Password.CreateHash(this.Text.Trim());
    }
  }
}
