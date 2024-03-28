using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static System.Net.WebRequestMethods;

namespace Audio_Plyer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CommonOpenFileDialog music = new CommonOpenFileDialog { IsFolderPicker = true };
        private bool flag = true;
        private bool VolumeFlag = true;
        private bool RepeatFlag = false;
        private bool ReverseFlag = false;
        string[] files;
        private double rememberVolume;
        public MainWindow()
        {
            InitializeComponent();
            media.Volume = 0.3;
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            timer.Start();
            AudioSl.Maximum = media.NaturalDuration.TimeSpan.Ticks;

            Button[] buttons = new Button[] { RepeatBTN, PlayOrPauseBTN, PlayNextBTN, ReverseBTN, VolumeBTN };
            Slider[] slider = new Slider[] { AudioSl, VolumeSl };

            foreach (var btn in buttons) btn.IsEnabled = true;
            foreach (var sld in slider) sld.IsEnabled = true;

            IconOfState.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;

/*            AudioSl.Maximum = media.NaturalDuration.TimeSpan.Seconds;
*/        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            AudioSl.Value = media.Position.Ticks;

            if (media.NaturalDuration.HasTimeSpan)
            {
                if (media.Position.Seconds < 10)
                {
                    CurrentTime.Text = media.Position.Minutes.ToString() + ":0" + media.Position.Seconds.ToString();
                    RemainedTime.Text = (media.NaturalDuration.TimeSpan.Minutes - media.Position.Minutes).ToString() + ":" + (media.NaturalDuration.TimeSpan.Seconds - media.Position.Seconds).ToString();
                }
                else if (media.NaturalDuration.TimeSpan.Seconds - media.Position.Seconds < 10)
                {
                    CurrentTime.Text = media.Position.Minutes.ToString() + ":" + media.Position.Seconds.ToString();
                    RemainedTime.Text = (media.NaturalDuration.TimeSpan.Minutes - media.Position.Minutes).ToString() + ":" + (media.NaturalDuration.TimeSpan.Seconds - media.Position.Seconds).ToString();
                }
                else
                {
                    CurrentTime.Text = media.Position.Minutes.ToString() + ":" + media.Position.Seconds.ToString();
                    RemainedTime.Text = (media.NaturalDuration.TimeSpan.Minutes - media.Position.Minutes).ToString() + ":" + (media.NaturalDuration.TimeSpan.Seconds - media.Position.Seconds).ToString();
                }
            }
        }

        private void AudioSl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = new TimeSpan(Convert.ToInt64(AudioSl.Value));
        }
        private void FolderBTN_Click(object sender, RoutedEventArgs e)
        {
            CommonFileDialogResult result = music.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                files = Directory.GetFiles(music.FileName);

                MusicOrderLBX.Items.Clear();
                Fill_LBX();

                media.Source = new Uri(music.FileName + "\\" + MusicOrderLBX.Items[0]);
                media.Play();
                IconOfState.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                flag = !flag;
                MusicOrderLBX.SelectedIndex = 0;
            }
        }
        private void Fill_LBX()
        {
            foreach (string file in files)
            {
                string[] FindName = file.Split('\\');
                string FileName = FindName[FindName.Count() - 1];
                if (FileName.EndsWith(".mp3") || FileName.EndsWith(".m4a") || FileName.EndsWith(".wav"))
                {
                    MusicOrderLBX.Items.Add(FileName);
                }
            }
        }

        private void VolumeSl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)  // завершено
        {
            media.Volume = VolumeSl.Value;
            if (media.Volume == 0)
            {
                VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeOff;
                VolumeIcon.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh;
                VolumeIcon.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void PlayOrPauseBTN_Click(object sender, RoutedEventArgs e)   // завершено
        {
            if (flag)
            {
                IconOfState.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                media.Play();
            }
            else
            {
                IconOfState.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                media.Pause();
            }
            flag = !flag;
        }
        private void VolumeBTN_Click(object sender, RoutedEventArgs e) // завершено
        {
            if (VolumeFlag)
            {
                VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeOff;
                VolumeIcon.Foreground = new SolidColorBrush(Colors.Red);
                rememberVolume = media.Volume;
                VolumeSl.Value = 0;
                media.Volume = 0;
            }
            else
            {
                VolumeIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh;
                VolumeIcon.Foreground = new SolidColorBrush(Colors.White);
                VolumeSl.Value = rememberVolume;
                media.Volume = rememberVolume;
            }
            VolumeFlag = !VolumeFlag;
        }

        private void ReverseBTN_Click(object sender, RoutedEventArgs e)
        {
            
            if (!ReverseFlag)
            {
                ReverseBTN.Background = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                MusicOrderLBX.Items.Clear();
                Fill_LBX();
            }
            else
            {
                ReverseBTN.Background = new SolidColorBrush(Colors.DarkSlateGray);
                string[] ReverseLBX = (from object item in MusicOrderLBX.Items select item.ToString()).ToArray<string>();
           
                Random rand = new Random();
                for (int i = ReverseLBX.Length - 1; i > 0; i--)
                {
                    int j = rand.Next(i);
                    string tmp = ReverseLBX[i];
                    ReverseLBX[i] = ReverseLBX[j];
                    ReverseLBX[j] = tmp;
                }

                MusicOrderLBX.Items.Clear();
                foreach (var song in ReverseLBX) MusicOrderLBX.Items.Add(song);
            }
            ReverseFlag = !ReverseFlag;
        }

        private void MusicOrderLBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string Name = MusicOrderLBX.SelectedValue as string;
            media.Source = new Uri(music.FileName + "\\" + Name);
            AudioSl.Value = 0;
            media.Play();
            CheckButtons();
        }
        private void CheckButtons()
        {
            if (MusicOrderLBX.SelectedIndex - 1 >= 0) PlayPreviousBTN.IsEnabled = true;
            if (MusicOrderLBX.SelectedIndex + 1 != MusicOrderLBX.Items.Count) PlayNextBTN.IsEnabled = false;
        }
        private void RepeatBTN_Click(object sender, RoutedEventArgs e)
        {
            if (!RepeatFlag)
            {
                RepeatBTN.Background = new SolidColorBrush(Colors.DarkSlateGray);
            }
            else
            {
                RepeatBTN.Background = new SolidColorBrush(Color.FromRgb(40,40,40));
            }
            RepeatFlag = !RepeatFlag;
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (RepeatFlag)
            {
                media.Stop();
                Thread.Sleep(1000);
                AudioSl.Value = 0;
                media.Play();
            }
            else MusicOrderLBX.SelectedIndex += 1;
        }

        private void PlayNextBTN_Click(object sender, RoutedEventArgs e)
        {
            MusicOrderLBX.SelectedIndex += 1;
            CheckButtons();
        }

        private void PlayPreviousBTN_Click(object sender, RoutedEventArgs e)
        {
            MusicOrderLBX.SelectedIndex -= 1;
            CheckButtons();
        }

        private void ShowWindow1(List<string> history)
        {
            Window1 Window1 = new Window1();
            Window1.UpdateHistoryListBox(history);

           
        }

        private void Open_history(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
