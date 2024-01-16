// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.TextBoxEx
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class TextBoxEx : TextBox, IPromptText, IDelayedAutoCompleteList
  {
    private bool quickSelectAll;
    private string promptText;
    private bool autoCompleteInitialized;
    private Func<AutoCompleteStringCollection> autoCompletePredicate;
    private bool hasEdited;
    private bool emptyText;

    public TextBoxEx() => this.FocusSelect = true;

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("Appearance")]
    [Description("The prompt text to display when there is nothing in the Text property.")]
    [DefaultValue(null)]
    public string PromptText
    {
      get => this.promptText;
      set
      {
        this.promptText = value;
        if (!this.IsHandleCreated)
          return;
        this.SetPromptText();
      }
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("Behavior")]
    [Description("Automatically select the text when control receives the focus.")]
    [DefaultValue(true)]
    public bool FocusSelect { get; set; }

    protected override bool IsInputKey(Keys keyData)
    {
      return !this.Multiline && (keyData == Keys.Down || keyData == Keys.Up) || base.IsInputKey(keyData);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      this.SetPromptText();
    }

    protected override void OnEnter(EventArgs e)
    {
      this.DoEnter();
      base.OnEnter(e);
    }

    private void DoEnter()
    {
      this.emptyText = false;
      this.InitializeAutoComplete(true);
      if (string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(this.PromptText))
      {
        this.emptyText = true;
        this.Text = this.PromptText;
      }
      if (this.Text.Length > 0 && this.FocusSelect)
      {
        this.SelectAll();
        this.quickSelectAll = true;
      }
      this.hasEdited = false;
    }

    private void InitializeAutoComplete(bool withoutFocus = false, bool delayed = false)
    {
      if (this.autoCompletePredicate == null || this.autoCompleteInitialized || !(this.Focused | withoutFocus))
        return;
      this.autoCompleteInitialized = true;
      this.AutoCompleteCustomSource = this.autoCompletePredicate();
    }

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      this.hasEdited = true;
    }

    protected override void OnLeave(EventArgs e)
    {
      base.OnLeave(e);
      this.DoLeave();
    }

    private void DoLeave()
    {
      if (!string.IsNullOrEmpty(this.PromptText) && !this.hasEdited && this.emptyText && this.SelectedText == this.PromptText)
        this.Text = string.Empty;
      this.quickSelectAll = false;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      if (!this.quickSelectAll)
        return;
      this.SelectAll();
    }

    protected override void OnKeyDown(KeyEventArgs e) => this.quickSelectAll = false;

    protected override void OnMouseUp(MouseEventArgs mevent) => this.quickSelectAll = false;

    private void SetPromptText()
    {
      if (this.Focused)
        this.DoLeave();
      this.SetCueText(this.promptText);
      if (!this.Focused)
        return;
      this.DoEnter();
    }

    public void SetLazyAutoComplete(
      Func<AutoCompleteStringCollection> autoCompletePredicate)
    {
      this.autoCompletePredicate = autoCompletePredicate;
      this.AutoCompleteMode = AutoCompleteMode.Append;
      this.AutoCompleteSource = AutoCompleteSource.CustomSource;
      this.AutoCompleteCustomSource = (AutoCompleteStringCollection) null;
      this.ResetLazyAutoComplete();
    }

    public void ResetLazyAutoComplete()
    {
      this.autoCompleteInitialized = false;
      this.InitializeAutoComplete();
    }

    public void BuildAutoComplete() => this.InitializeAutoComplete(true);
  }
}
