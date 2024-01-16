// Decompiled with JetBrains decompiler
// Type: cYo.Common.Windows.Forms.KeySequenceHandler
// Assembly: cYo.Common.Windows, Version=1.0.5915.38774, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 242774FD-C08F-4377-963D-D9AB7AE652C7
// Assembly location: C:\Program Files\ComicRack\cYo.Common.Windows.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

#nullable disable
namespace cYo.Common.Windows.Forms
{
  public class KeySequenceHandler : Component
  {
    private Control control;
    private readonly KeySequenceCollection sequences = new KeySequenceCollection();
    private int intervallTime = 1000;
    private readonly List<KeySequenceHandler.SequenceState> activeSequences = new List<KeySequenceHandler.SequenceState>();
    private Keys keyState;
    private IContainer components;

    public KeySequenceHandler() => this.InitializeComponent();

    public KeySequenceHandler(IContainer container)
      : this()
    {
      container.Add((IComponent) this);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.components != null)
          this.components.Dispose();
        this.Control = (Control) null;
      }
      base.Dispose(disposing);
    }

    [DefaultValue(null)]
    public Control Control
    {
      get => this.control;
      set
      {
        if (this.control == value)
          return;
        if (this.control != null)
        {
          this.control.KeyDown -= new KeyEventHandler(this.control_KeyDown);
          this.control.KeyUp -= new KeyEventHandler(this.control_KeyUp);
        }
        this.control = value;
        if (this.control == null)
          return;
        this.control.KeyDown += new KeyEventHandler(this.control_KeyDown);
        this.control.KeyUp += new KeyEventHandler(this.control_KeyUp);
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public KeySequenceCollection Sequences => this.sequences;

    public KeySequenceCollection ActiveSequences
    {
      get
      {
        KeySequenceCollection activeSequences = new KeySequenceCollection();
        foreach (KeySequenceHandler.SequenceState activeSequence in this.activeSequences)
          activeSequences.Add(activeSequence.Sequence);
        return activeSequences;
      }
    }

    [DefaultValue(1000)]
    public int IntervallTime
    {
      get => this.intervallTime;
      set => this.intervallTime = value;
    }

    private void ParseKey(Keys key)
    {
      for (int index = this.activeSequences.Count - 1; index >= 0; --index)
      {
        KeySequenceHandler.SequenceState activeSequence = this.activeSequences[index];
        if (!activeSequence.Parse(key, this.intervallTime))
          this.activeSequences.RemoveAt(index);
        else if (activeSequence.Completed)
        {
          this.activeSequences.Clear();
          this.OnSequenceCompleted(activeSequence.Sequence);
          return;
        }
      }
      foreach (KeySequence sequence in (List<KeySequence>) this.Sequences)
      {
        KeySequence ksrun = sequence;
        if (this.activeSequences.Find((Predicate<KeySequenceHandler.SequenceState>) (ss => ss.Sequence == ksrun)) == null)
        {
          KeySequenceHandler.SequenceState sequenceState = new KeySequenceHandler.SequenceState(sequence);
          if (sequenceState.Parse(key, this.intervallTime))
            this.activeSequences.Add(sequenceState);
        }
      }
    }

    public void Reset() => this.activeSequences.Clear();

    public event KeyEventHandler KeyDown;

    public event KeyEventHandler KeyUp;

    public event EventHandler<KeySequenceEventArgs> SequenceCompleted;

    private void FireSequenceEvent(
      EventHandler<KeySequenceEventArgs> eventHandler,
      KeySequence keySequence)
    {
      if (eventHandler == null)
        return;
      eventHandler((object) this, new KeySequenceEventArgs(keySequence));
    }

    protected virtual void OnSequenceCompleted(KeySequence sequence)
    {
      this.FireSequenceEvent(this.SequenceCompleted, sequence);
    }

    protected virtual void OnKeyDown(KeyEventArgs e)
    {
      if (this.KeyDown == null)
        return;
      this.KeyDown((object) this, e);
    }

    protected virtual void OnKeyUp(KeyEventArgs e)
    {
      this.ParseKey(e.KeyCode | this.keyState);
      if (this.KeyUp == null)
        return;
      this.KeyUp((object) this, e);
    }

    private void control_KeyDown(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.ShiftKey:
        case Keys.LShiftKey:
          this.keyState |= Keys.Shift;
          break;
        case Keys.ControlKey:
        case Keys.LControlKey:
          this.keyState |= Keys.Control;
          break;
        case Keys.Menu:
        case Keys.LMenu:
          this.keyState |= Keys.Menu;
          break;
        default:
          this.OnKeyDown(e);
          break;
      }
    }

    private void control_KeyUp(object sender, KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.ShiftKey:
        case Keys.LShiftKey:
          this.keyState &= ~Keys.Shift;
          break;
        case Keys.ControlKey:
        case Keys.LControlKey:
          this.keyState &= ~Keys.Control;
          break;
        case Keys.Menu:
        case Keys.LMenu:
          this.keyState &= ~Keys.Menu;
          break;
        default:
          this.OnKeyUp(e);
          break;
      }
    }

    private void InitializeComponent() => this.components = (IContainer) new System.ComponentModel.Container();

    private class SequenceState
    {
      private int position;
      private DateTime lastKeyEntered = DateTime.Now;

      public SequenceState(KeySequence sequence) => this.Sequence = sequence;

      public KeySequence Sequence { get; set; }

      public int Position
      {
        get => this.position;
        set
        {
          if (this.position == value)
            return;
          this.position = value;
          this.lastKeyEntered = DateTime.Now;
        }
      }

      public DateTime LastKeyEntered => this.lastKeyEntered;

      public Keys NextKey => this.Sequence.Sequence[this.position];

      public bool Completed => this.position >= this.Sequence.Sequence.Count;

      public bool Parse(Keys key, int maximumIntervall)
      {
        if ((DateTime.Now - this.LastKeyEntered).TotalMilliseconds > (double) maximumIntervall || this.NextKey != key)
          return false;
        ++this.Position;
        return true;
      }
    }
  }
}
