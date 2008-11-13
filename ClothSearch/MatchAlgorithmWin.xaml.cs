using System.Windows;
using Zju.View;

namespace ClothSearch
{
    /// <summary>
    /// Interaction logic for SearchOptionsWin.xaml
    /// </summary>
    public partial class MatchAlgorithmWin : Window
    {
        private AlgorithmDesc aDesc;

        public MatchAlgorithmWin(AlgorithmDesc aDesc)
        {
            InitializeComponent();

            this.aDesc = aDesc;
            rbtnTexture3.IsChecked = true;
        }

        private void btnOptionSave_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = 0;
            if (true == rbtnColor1.IsChecked)
            {
                selectedIndex = cmbColor1.SelectedIndex;
                aDesc.AType = AlgorithmType.Color1;
            }
            else if (true == rbtnTexture1.IsChecked)
            {
                selectedIndex = cmbRecall1.SelectedIndex;
                aDesc.AType = AlgorithmType.Texture1;
            }
            else if (true == rbtnTexture2.IsChecked)
            {
                selectedIndex = cmbRecall2.SelectedIndex;
                aDesc.AType = AlgorithmType.Texture2;
            }
            else
            {
                // default
                selectedIndex = cmbRecall3.SelectedIndex;
                aDesc.AType = AlgorithmType.Texture3;
            }

            RecallLevel[] rLevels = new RecallLevel[4] { RecallLevel.Default, RecallLevel.Recall1, RecallLevel.Recall2, RecallLevel.Recall3 };
            aDesc.RLevel = rLevels[selectedIndex];

            this.Close();
        }

        private void btnOptionCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
