using System.Windows;
using Zju.View;
using System.Windows.Controls;

namespace ClothSearch
{
    /// <summary>
    /// Interaction logic for SearchOptionsWin.xaml
    /// </summary>
    public partial class MatchAlgorithmWin : Window
    {
        private AlgorithmDesc aDesc;

        private RecallLevel[] rLevels;

        private AlgorithmType[] aTypes;

        private RadioButton[] rbtns;

        private ComboBox[] cmbs;

        public MatchAlgorithmWin(AlgorithmDesc aDesc)
        {
            InitializeComponent();

            this.aDesc = aDesc;

            // the first elements of these arrays are always the default ones.
            rLevels = new RecallLevel[] { RecallLevel.Default, RecallLevel.Recall1, RecallLevel.Recall2, RecallLevel.Recall3 };
            aTypes = new AlgorithmType[] { AlgorithmType.Texture3, AlgorithmType.Texture2, AlgorithmType.Texture1, AlgorithmType.Color1 };
            rbtns = new RadioButton[] { rbtnTexture3, rbtnTexture2, rbtnTexture1, rbtnColor1 };
            cmbs = new ComboBox[] { cmbTexture3, cmbTexture2, cmbTexture1, cmbColor1 };


            int rLevelIndex = 0;
            for (int i = 0; i < rLevels.Length; ++i)
            {
                if (aDesc.RLevel == rLevels[i])
                {
                    rLevelIndex = i;
                    break;
                }
            }

            int aTypeIndex = 0;
            for (int i = 0; i < aTypes.Length; ++i)
            {
                if (aDesc.AType == aTypes[i])
                {
                    aTypeIndex = i;
                    break;
                }
            }

            rbtns[aTypeIndex].IsChecked = true;
            cmbs[aTypeIndex].SelectedIndex = rLevelIndex;
        }

        private void btnOptionSave_Click(object sender, RoutedEventArgs e)
        {
            int aTypeIndex = 0;
            for (int i = 0; i < rbtns.Length; ++i)
            {
                if (true == rbtns[i].IsChecked)
                {
                    aTypeIndex = i;
                    break;
                }
            }

            aDesc.AType = aTypes[aTypeIndex];
            aDesc.RLevel = rLevels[cmbs[aTypeIndex].SelectedIndex];

            this.Close();
        }

        private void btnOptionCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
