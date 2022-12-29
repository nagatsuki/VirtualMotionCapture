﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using UnityMemoryMappedFile;

namespace VirtualMotionCaptureControlPanel
{
    /// <summary>
    /// CalibrationResultWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CalibrationResultWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        int timerCount = 0;
        bool isShown = false;
        bool timerEnable = true;
        public CalibrationResultWindow()
        {
            InitializeComponent();
        }

        public void Update(PipeCommands.CalibrationResult result) {
            switch (result.Type) {
                case PipeCommands.CalibrateType.Ipose:
                case PipeCommands.CalibrateType.Tpose:
                    {
                        CalibrationResult_Caption1TextBlock.Background = new SolidColorBrush(Color.FromRgb(230,255,230));
                        CalibrationResult_Caption1TextBlock.Text = "ℹ キャリブレーションが完了しました";
                        CalibrationResult_Caption2TextBlock.Text = "あなたの身体パラメータを推定しました。";

                        DetailMessageTextBlock.Text = "身長: "+String.Format("{0:0.0}cm",result.UserHeight*100) + "\n"+"実際の値と大きく違う場合、装着位置や床の高さ設定を確認し、正しい姿勢でもう一度キャリブレーションを実行してください。";

                        DetailMessageContentControl.Visibility = Visibility.Visible;
                        DetailMessageHideTextBlock.Visibility = Visibility.Visible;
                        DetailMessageTextBlock.Visibility = Visibility.Collapsed;

                        timerEnable = true;
                        TimerCloseTextBlock.Visibility = Visibility.Visible;
                        break;
                    }
                case PipeCommands.CalibrateType.FixedHand:
                case PipeCommands.CalibrateType.FixedHandWithGround:
                    {
                        CalibrationResult_Caption1TextBlock.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                        CalibrationResult_Caption1TextBlock.Text = "ℹ キャリブレーションが完了しました";
                        CalibrationResult_Caption2TextBlock.Text = "";

                        DetailMessageTextBlock.Text = "";

                        DetailMessageContentControl.Visibility = Visibility.Hidden;
                        DetailMessageHideTextBlock.Visibility = Visibility.Collapsed;
                        DetailMessageTextBlock.Visibility = Visibility.Collapsed;

                        timerEnable = true;
                        TimerCloseTextBlock.Visibility = Visibility.Visible;
                        break;
                    }
                case PipeCommands.CalibrateType.Invalid:
                default:
                    {
                        CalibrationResult_Caption1TextBlock.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                        CalibrationResult_Caption1TextBlock.Text = "⚠ キャリブレーションに失敗しました";
                        CalibrationResult_Caption2TextBlock.Text = "デバイスの認識や割り当て設定を確認してください";

                        DetailMessageTextBlock.Text = result.Message;

                        DetailMessageContentControl.Visibility = Visibility.Visible;
                        DetailMessageHideTextBlock.Visibility = Visibility.Collapsed;
                        DetailMessageTextBlock.Visibility = Visibility.Visible;

                        timerEnable = false;
                        TimerCloseTextBlock.Visibility = Visibility.Hidden;
                        break;
                    }
            }

            timerCount = 10;
            TimerCloseTextBlock.Text = timerCount + "秒後に自動で閉じます";

            isShown = true;
            this.Show();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            hideClose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Interval = new TimeSpan(0,0,1);
            timer.Tick += new EventHandler((object sender_e, EventArgs ee) => {
                if (timerEnable) {
                    if (timerCount == 0)
                    {
                        if (isShown)
                        {
                            hideClose();
                        }
                    }
                    else
                    {
                        timerCount--;
                        TimerCloseTextBlock.Text = timerCount + "秒後に自動で閉じます";
                    }
                }
            });
            timer.Start();
        }

        private void DetailMessageContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DetailMessageHideTextBlock.Visibility = Visibility.Collapsed;
            DetailMessageTextBlock.Visibility = Visibility.Visible;
            TimerCloseTextBlock.Visibility = Visibility.Hidden;
            timerEnable = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            hideClose();
        }

        void hideClose() {
            isShown = false;
            this.Hide();
        }
    }
}
