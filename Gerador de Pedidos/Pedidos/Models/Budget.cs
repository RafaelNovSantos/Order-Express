using System.ComponentModel;

public class Budget : INotifyPropertyChanged
{
    private string _valorTotal;

    public string Valor_Total
    {
        get => _valorTotal;
        set
        {
            if (_valorTotal != value)
            {
                _valorTotal = value;
                OnPropertyChanged(nameof(Valor_Total));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
