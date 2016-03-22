using Android.Views;
using Android.Widget;

namespace PasswordGenerator.Android.Activities
{
    public partial class MainActivity
    {

        private Button _btnGenerate;
        private Button _btnSubmit;
        private Button _btnCopy;

        private ViewGroup _lytOptionExpand;
        private TableLayout _tlytSignals;

        private TextView _tvResult;
        private TextView _tvSimpleKey;

        private CheckBox _cbUpper;
        private CheckBox _cbLower;
        private CheckBox _cbNum;
        private CheckBox _cbSignals;

        private RadioGroup _rGroupUpper;
        private RadioGroup _rGroupLower;
        private RadioGroup _rGroupNum;
        private RadioGroup _rGroupSignals;

        private EditText _txtKeywords;
        private EditText _txtLength;
        private EditText _txtSimpleKey;

        private void Definition()
        {
            _btnGenerate = FindViewById<Button>(Resource.Id.btnGenerate);
            _btnSubmit = FindViewById<Button>(Resource.Id.btnSubmit);
            _btnCopy = FindViewById<Button>(Resource.Id.btnCopy);

            _tvResult = FindViewById<TextView>(Resource.Id.tvResult);
            _tvSimpleKey = FindViewById<TextView>(Resource.Id.tvSimpleKey);

            _lytOptionExpand = FindViewById<ViewGroup>(Resource.Id.lytOptionExpand);
            _tlytSignals = FindViewById<TableLayout>(Resource.Id.tlytSignals);

            _cbUpper = FindViewById<CheckBox>(Resource.Id.cbUpper);
            _cbLower = FindViewById<CheckBox>(Resource.Id.cbLower);
            _cbNum = FindViewById<CheckBox>(Resource.Id.cbNum);
            _cbSignals = FindViewById<CheckBox>(Resource.Id.cbSignals);

            _rGroupUpper = FindViewById<RadioGroup>(Resource.Id.rGroupUpper);
            _rGroupLower = FindViewById<RadioGroup>(Resource.Id.rGroupLower);
            _rGroupNum = FindViewById<RadioGroup>(Resource.Id.rGroupNum);
            _rGroupSignals = FindViewById<RadioGroup>(Resource.Id.rGroupSignals);

            _txtKeywords = FindViewById<EditText>(Resource.Id.txtKeywords);
            _txtLength = FindViewById<EditText>(Resource.Id.txtLength);
            _txtSimpleKey = FindViewById<EditText>(Resource.Id.txtSimpleKey);
        }
    }
}