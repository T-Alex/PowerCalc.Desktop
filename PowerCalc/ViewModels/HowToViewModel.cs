using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TAlex.PowerCalc.Services;
using TAlex.WPF.Mvvm;
using TAlex.WPF.Mvvm.Commands;


namespace TAlex.PowerCalc.ViewModels
{
    public class HowToViewModel : ViewModelBase
    {
        #region Fields

        protected readonly IAppSettings AppSettings;

        protected IList<HowToItem> Items { get; set; }

        private HowToItem _currentItem;
        private int _currentIndex;

        #endregion

        #region Properties

        public bool ShowOnStartup
        {
            get
            {
                return AppSettings.ShowHowToOnStartup;
            }

            set
            {
                AppSettings.ShowHowToOnStartup = value;
            }
        }

        public HowToItem CurrentItem
        {
            get
            {
                return _currentItem;
            }

            private set
            {
                Set(() => CurrentItem, ref _currentItem, value);
            }
        }

        public int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }

            private set
            {
                Set(() => CurrentIndex, ref _currentIndex, value);
                CurrentItem = Items[value - 1];
            }
        }

        public int TotalItems
        {
            get
            {
                return Items.Count;
            }
        }

        #endregion

        #region Commands

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        #endregion

        #region Constructors

        public HowToViewModel(IAppSettings appSettings, IHowToItemsProvider howToItemsProvider)
        {
            InitializeCommands();

            AppSettings = appSettings;
            ShowOnStartup = appSettings.ShowHowToOnStartup;

            Items = howToItemsProvider.GetItems();
            CurrentIndex = new Random().Next(1, Items.Count + 1);
        }

        #endregion

        #region Methods

        protected virtual void InitializeCommands()
        {
            PreviousCommand = new RelayCommand(Previous);
            NextCommand = new RelayCommand(Next);
        }


        private void Previous()
        {
            CurrentIndex = (CurrentIndex - 1 < 1) ? Items.Count : CurrentIndex - 1;
        }

        private void Next()
        {
            CurrentIndex = (CurrentIndex + 1 > Items.Count) ? 1 : CurrentIndex + 1;
        }

        #endregion
    }

    public class HowToItem
    {
        public virtual string Caption { get; set; }
        public virtual object Body { get; set; }
    }
}
