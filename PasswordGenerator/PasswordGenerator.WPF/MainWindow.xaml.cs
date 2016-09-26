using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using PasswordGenerator.Core;
using PasswordGenerator.Core.Enums;
using PasswordGenerator.Core.Models;

namespace PasswordGenerator.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 是否已初始化完成
        /// </summary>
        private readonly bool _initCompleted;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //刷新当前模式
            _initCompleted = true;
            RefreshModes();
        }

        #region 事件

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var keywords = txtKeywords.Password;
            var length = int.Parse(txtLength.Text);
            var key = lblSimpleKey.Content.ToString();

            lblResult.Content = Generator.Generate(keywords, length, key);
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            //复制
            Clipboard.SetText(lblResult.Content.ToString());
            MessageBox.Show("已复制", "复制成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var modeStateHex = txtSimpleKey.Text;
            if (string.IsNullOrEmpty(modeStateHex)) return;

            var modeStateOct = Generator.ModeStateHexToOct(modeStateHex);
            var modeStates = Generator.GetModesFromOct(modeStateOct);

            //将输入的模式简码转换到当前选项中
            foreach (var modeState in modeStates)
            {
                switch (modeState.RangeType)
                {
                    case EnumValueRangeType.UpperWord:
                        ((RadioButton)dpUpper.Children[0]).IsChecked = modeState.State == EnumChooseState.Must;
                        cbUpper.IsChecked = modeState.State != EnumChooseState.None;
                        break;
                    case EnumValueRangeType.LowerWord:
                        ((RadioButton)dpLower.Children[0]).IsChecked = modeState.State == EnumChooseState.Must;
                        cbLower.IsChecked = modeState.State != EnumChooseState.None;
                        break;
                    case EnumValueRangeType.Number:
                        ((RadioButton)dpNum.Children[0]).IsChecked = modeState.State == EnumChooseState.Must;
                        cbNum.IsChecked = modeState.State != EnumChooseState.None;
                        break;
                    case EnumValueRangeType.Signal:
                        ((RadioButton)dpSignal.Children[0]).IsChecked = modeState.State == EnumChooseState.Must;
                        cbSignal.IsChecked = modeState.State != EnumChooseState.None;
                        gridSignals.Visibility = modeState.State == EnumChooseState.None ? Visibility.Collapsed : Visibility.Visible;

                        var chosenSignals = Generator.GetSignalsFromOct(modeStateOct);
                        foreach (ToggleButton child in gridSignals.Children)
                        {
                            child.IsChecked = chosenSignals.Contains(child.Content.ToString());
                        }
                        break;
                    default:
                        break;
                }
            }

            lblSimpleKey.Content = txtSimpleKey.Text;
        }

        private void cbUpper_Click(object sender, RoutedEventArgs e)
        {
            if (cbUpper.IsChecked != null && cbUpper.IsChecked.Value)
            {
                dpUpper.IsEnabled = true;
            }
            else
            {
                dpUpper.IsEnabled = false;
            }

            RefreshModes();
        }

        private void cbLower_Click(object sender, RoutedEventArgs e)
        {
            if (cbLower.IsChecked != null && cbLower.IsChecked.Value)
            {
                dpLower.IsEnabled = true;
            }
            else
            {
                dpLower.IsEnabled = false;
            }

            RefreshModes();
        }

        private void cbNum_Click(object sender, RoutedEventArgs e)
        {
            if (cbNum.IsChecked != null && cbNum.IsChecked.Value)
            {
                dpNum.IsEnabled = true;
            }
            else
            {
                dpNum.IsEnabled = false;
            }

            RefreshModes();
        }

        private void cbSignal_Click(object sender, RoutedEventArgs e)
        {
            if (cbSignal.IsChecked != null && cbSignal.IsChecked.Value)
            {
                dpSignal.IsEnabled = true;
                gridSignals.Visibility = Visibility.Visible;
            }
            else
            {
                dpSignal.IsEnabled = false;
                gridSignals.Visibility = Visibility.Collapsed;
            }

            RefreshModes();
        }

        private void RadioCheckedChange(object sender, RoutedEventArgs e)
        {
            RefreshModes();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 刷新当前模式
        /// </summary>
        private void RefreshModes()
        {
            if (!_initCompleted) return;

            StringBuilder keySBuilder = new StringBuilder();
            GetModeState(cbUpper, dpUpper, keySBuilder);
            GetModeState(cbLower, dpLower, keySBuilder);
            GetModeState(cbNum, dpNum, keySBuilder);
            GetModeState(cbSignal, dpSignal, keySBuilder);

            //子符号的状态获取
            char[] signals = { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };
            if (cbSignal.IsChecked.GetValueOrDefault())
            {
                foreach (ToggleButton child in gridSignals.Children)
                {
                    var index = Key.Signals.IndexOf(child.Content.ToString(), StringComparison.Ordinal);
                    if (child.IsChecked.GetValueOrDefault())
                    {
                        signals[index] = '1';
                    }
                }
            }
            keySBuilder.Append(signals);

            lblSimpleKey.Content = Generator.ModeStateOctToHex(keySBuilder.ToString());
        }

        /// <summary>
        /// 获取模式状态
        /// </summary>
        /// <param name="cb">启用控制的复选框</param>
        /// <param name="dp">可选和必选所在的容器</param>
        /// <param name="sb">输出字符串构建器</param>
        private void GetModeState(CheckBox cb, DockPanel dp, StringBuilder sb)
        {
            var state = EnumChooseState.None;
            if (cb.IsChecked.GetValueOrDefault())
            {
                var cbOption = dp.Children[0] as RadioButton;
                //第一个选项是“必须”选项，如果不是则不正常，默认按可选
                if (cbOption != null && cbOption.Content.ToString() == "Must")
                {
                    state = cbOption.IsChecked.GetValueOrDefault() ? EnumChooseState.Must : EnumChooseState.Optional;
                }
                else
                {
                    state = EnumChooseState.Optional;
                }
            }

            //保证输出长度为两位
            var octStr = Convert.ToString((short)state, 2);
            if (octStr.Length < 2)
            {
                sb.Append("0");
            }
            sb.Append(octStr);
        }

        #endregion

    }
}
