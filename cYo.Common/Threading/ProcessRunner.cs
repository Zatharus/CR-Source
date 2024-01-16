// Decompiled with JetBrains decompiler
// Type: cYo.Common.Threading.ProcessRunner
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using cYo.Common.ComponentModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

#nullable disable
namespace cYo.Common.Threading
{
  public class ProcessRunner : DisposableObject
  {
    private Process currentProcess;
    private ProcessPriorityClass priority = ProcessPriorityClass.Normal;
    private readonly byte[] readBuffer = new byte[1024];
    private Stream readStream;

    protected override void Dispose(bool disposing) => this.Stop();

    public ProcessPriorityClass Priority
    {
      get => this.priority;
      set
      {
        if (this.priority == value)
          return;
        if (this.currentProcess != null)
          this.currentProcess.PriorityClass = value;
        this.priority = value;
      }
    }

    public bool IsRunning
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.currentProcess != null;
      }
    }

    public DateTime StartTime
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return this.currentProcess.StartTime;
      }
    }

    public TimeSpan RunningTime
    {
      get
      {
        using (ItemMonitor.Lock((object) this))
          return DateTime.Now - this.StartTime;
      }
    }

    public void Run(string path, string arguments, bool waitForExit)
    {
      this.Stop();
      ProcessStartInfo processStartInfo = new ProcessStartInfo(path, arguments)
      {
        CreateNoWindow = true,
        RedirectStandardOutput = true,
        WorkingDirectory = Path.GetDirectoryName(path),
        UseShellExecute = false
      };
      this.currentProcess = new Process()
      {
        StartInfo = processStartInfo,
        EnableRaisingEvents = true
      };
      this.currentProcess.Exited += new EventHandler(this.ProcessStopped);
      this.currentProcess.Start();
      this.currentProcess.PriorityClass = this.priority;
      this.readStream = this.currentProcess.StandardOutput.BaseStream;
      this.readStream.BeginRead(this.readBuffer, 0, this.readBuffer.Length, new AsyncCallback(this.ReadOutput), (object) null);
      if (!waitForExit)
        return;
      this.currentProcess.WaitForExit();
      this.Stop();
    }

    public void Run(string path, string arguments) => this.Run(path, arguments, false);

    public void Stop()
    {
      if (!this.IsRunning)
        return;
      using (ItemMonitor.Lock((object) this))
      {
        try
        {
          this.currentProcess.Kill();
          this.currentProcess.WaitForExit();
        }
        catch
        {
        }
        finally
        {
          this.currentProcess.Exited -= new EventHandler(this.ProcessStopped);
          this.currentProcess.Dispose();
          this.currentProcess = (Process) null;
        }
      }
    }

    private void ReadOutput(IAsyncResult ar)
    {
      try
      {
        int count = this.readStream.EndRead(ar);
        if (count <= 0)
          return;
        ProcessRunnerOutputEventArgs poea = new ProcessRunnerOutputEventArgs(Encoding.UTF8.GetString(this.readBuffer, 0, count));
        this.OnParseOutput(poea);
        if (poea.Cancel)
          this.Stop();
        this.readStream.BeginRead(this.readBuffer, 0, this.readBuffer.Length, new AsyncCallback(this.ReadOutput), (object) null);
      }
      catch
      {
      }
    }

    private void ProcessStopped(object sender, EventArgs e) => this.OnStopped();

    public event EventHandler Stopped;

    public event EventHandler<ProcessRunnerOutputEventArgs> ParseOutput;

    protected virtual void OnStopped()
    {
      if (this.Stopped == null)
        return;
      this.Stopped((object) this, EventArgs.Empty);
    }

    protected virtual void OnParseOutput(ProcessRunnerOutputEventArgs poea)
    {
      if (this.ParseOutput == null)
        return;
      this.ParseOutput((object) this, poea);
    }

    public static int RunElevated(string file, string arguments)
    {
      ProcessStartInfo startInfo = new ProcessStartInfo(file);
      startInfo.Arguments = arguments;
      startInfo.UseShellExecute = true;
      startInfo.Verb = "runas";
      try
      {
        Process process = Process.Start(startInfo);
        process.WaitForExit();
        return process.ExitCode;
      }
      catch
      {
        return -1;
      }
    }
  }
}
