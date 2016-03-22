using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PasswordGenerator.Core;
using PasswordGenerator.Core.Enums;
using PasswordGenerator.Core.Models;

namespace PasswordGenerator.Android.Activities
{
    [Activity(Label = "PasswordGenerator", MainLauncher = true, Icon = "@drawable/icon")]
    public partial class MainActivity : Activity
    {
        private ClipboardManager _clipboardManager;

        private bool _initCompleted;

        private readonly ToggleButton[] _signalArray = new ToggleButton[12];

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Definition();
            Initialize();

            RefreshModes();
        }

        private void Initialize()
        {
            _clipboardManager = (ClipboardManager)GetSystemService(ClipboardService);

            _btnGenerate.Click += btnGenerate_Click;
            _btnSubmit.Click += btnSubmit_Click;
            _lytOptionExpand.Click += lytOptionExpand_Click;
            _btnCopy.Click += btnCopy_Click;

            _cbUpper.Click += (o, e) =>
            {
                _rGroupUpper.Visibility = _cbUpper.Checked ? ViewStates.Visible : ViewStates.Gone;
                RefreshModes();
            };
            _cbLower.Click += (o, e) =>
            {
                _rGroupLower.Visibility = _cbLower.Checked ? ViewStates.Visible : ViewStates.Gone;
                RefreshModes();
            };
            _cbNum.Click += (o, e) =>
            {
                _rGroupNum.Visibility = _cbNum.Checked ? ViewStates.Visible : ViewStates.Gone;
                RefreshModes();
            };
            _cbSignals.Click += (o, e) =>
            {
                _rGroupSignals.Visibility = _cbSignals.Checked ? ViewStates.Visible : ViewStates.Gone;
                _tlytSignals.Visibility = _cbSignals.Checked ? ViewStates.Visible : ViewStates.Gone;

                RefreshModes();
            };
            _rGroupUpper.CheckedChange += (o, e) => { RefreshModes(); };
            _rGroupLower.CheckedChange += (o, e) => { RefreshModes(); };
            _rGroupNum.CheckedChange += (o, e) => { RefreshModes(); };
            _rGroupSignals.CheckedChange += (o, e) => { RefreshModes(); };

            InitSignalList();


            _initCompleted = true;
        }

        private void InitSignalList()
        {
            var signalIds = new[] {Resource.Id.tbtn1,
                Resource.Id.tbtn2,Resource.Id.tbtn3,Resource.Id.tbtn4,
                Resource.Id.tbtn5,Resource.Id.tbtn6,Resource.Id.tbtn7,
                Resource.Id.tbtn8,Resource.Id.tbtn9,Resource.Id.tbtn10,
                Resource.Id.tbtn11,Resource.Id.tbtn12};

            for (int i = 0; i < 12; i++)
            {
                var btn = FindViewById<ToggleButton>(signalIds[i]);
                btn.Click += (o, e) => { RefreshModes(); };
                _signalArray[i] = btn;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var keywords = _txtKeywords.Text;
            var length = int.Parse(_txtLength.Text);
            var key = _tvSimpleKey.Text;

            _tvResult.Text = Generator.Generate(keywords, length, key);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            var modeStateHex = _txtSimpleKey.Text;
            if (string.IsNullOrEmpty(modeStateHex)) return;

            var modeStateOct = Generator.ModeStateHexToOct(modeStateHex);
            var modeStates = Generator.GetModesFromOct(modeStateOct);

            //将输入的模式简码转换到当前选项中
            foreach (var modeState in modeStates)
            {
                switch (modeState.RangeType)
                {
                    case EnumValueRangeType.UpperWord:
                        _rGroupUpper.Check(modeState.State  == EnumChooseState.Must ? Resource.Id.rbUpper_Must : Resource.Id.rbUpper_Optional);
                        _cbUpper.Checked = modeState.State != EnumChooseState.None;
                        _rGroupUpper.Visibility = _cbUpper.Checked ? ViewStates.Visible : ViewStates.Gone;
                        break;
                    case EnumValueRangeType.LowerWord:
                        _rGroupLower.Check(modeState.State == EnumChooseState.Must ? Resource.Id.rbLower_Must : Resource.Id.rbLower_Optional);
                        _cbLower.Checked = modeState.State != EnumChooseState.None;
                        _rGroupLower.Visibility = _cbLower.Checked ? ViewStates.Visible : ViewStates.Gone;
                        break;
                    case EnumValueRangeType.Number:
                        _rGroupNum.Check(modeState.State == EnumChooseState.Must ? Resource.Id.rbNum_Must : Resource.Id.rbNum_Optional);
                        _cbNum.Checked = modeState.State != EnumChooseState.None;
                        _rGroupNum.Visibility = _cbNum.Checked ? ViewStates.Visible : ViewStates.Gone;
                        break;
                    case EnumValueRangeType.Signal:
                        _rGroupSignals.Check(modeState.State == EnumChooseState.Must ? Resource.Id.rbSignals_Must : Resource.Id.rbSignals_Optional);
                        _cbSignals.Checked = modeState.State != EnumChooseState.None;
                        _tlytSignals.Visibility = _rGroupSignals.Visibility = _cbSignals.Checked ? ViewStates.Visible : ViewStates.Gone;

                        var chosenSignals = Generator.GetSignalsFromOct(modeStateOct);
                        foreach (ToggleButton child in _signalArray)
                        {
                            child.Checked = chosenSignals.Contains(child.Text);
                        }
                        break;
                    default:
                        break;
                }
            }

            _tvSimpleKey.Text = _txtSimpleKey.Text;
        }

        private void lytOptionExpand_Click(object sender, EventArgs e)
        {
            var lytOptions = FindViewById<ViewGroup>(Resource.Id.lytOptions);
            var imgOptions = FindViewById<ImageView>(Resource.Id.imgOptions);
            if (lytOptions.Visibility == ViewStates.Visible)
            {
                lytOptions.Visibility = ViewStates.Gone;
                imgOptions.SetImageResource(Resource.Drawable.Expand);
            }
            else
            {
                lytOptions.Visibility = ViewStates.Visible;
                imgOptions.SetImageResource(Resource.Drawable.Collapse);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var data = ClipData.NewPlainText("text", _tvResult.Text);
            _clipboardManager.PrimaryClip = data;

            var dialog = new AlertDialog.Builder(this).Create();
            dialog.SetTitle("成功");
            dialog.SetMessage("复制成功");
            dialog.SetCanceledOnTouchOutside(true);
            dialog.Show();
        }


        #region 私有方法

        /// <summary>
        /// 刷新当前模式
        /// </summary>
        private void RefreshModes()
        {
            if (!_initCompleted) return;

            StringBuilder keySBuilder = new StringBuilder();
            GetModeState(_cbUpper, _rGroupUpper, keySBuilder);
            GetModeState(_cbLower, _rGroupLower, keySBuilder);
            GetModeState(_cbNum, _rGroupNum, keySBuilder);
            GetModeState(_cbSignals, _rGroupSignals, keySBuilder);

            //子符号的状态获取
            char[] signals = { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };
            if (_cbSignals.Checked)
            {
                foreach (ToggleButton child in _signalArray)
                {
                    var index = Key.Signals.IndexOf(child.Text, StringComparison.Ordinal);
                    if (child.Checked)
                    {
                        signals[index] = '1';
                    }
                }
            }
            keySBuilder.Append(signals);

            _tvSimpleKey.Text = Generator.ModeStateOctToHex(keySBuilder.ToString());
        }

        /// <summary>
        /// 获取模式状态
        /// </summary>
        /// <param name="cb">启用控制的复选框</param>
        /// <param name="rGroup">可选和必选所在的容器</param>
        /// <param name="sb">输出字符串构建器</param>
        private void GetModeState(CheckBox cb, RadioGroup rGroup, StringBuilder sb)
        {
            var state = EnumChooseState.None;
            if (cb.Checked)
            {
                var cbOption = FindViewById<RadioButton>(rGroup.CheckedRadioButtonId);
                //第一个选项是“必须”选项，如果不是则不正常，默认按可选
                if (rGroup.CheckedRadioButtonId != -1 && cbOption.Text == "Must")
                {
                    state = EnumChooseState.Must;
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

