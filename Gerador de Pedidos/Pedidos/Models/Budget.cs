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

    private int _numeroPedido;

    public int Numero_Pedido
    {
        get => _numeroPedido;
        set
        {
            if (_numeroPedido != value)
            {
                _numeroPedido = value;
                OnPropertyChanged(nameof(Numero_Pedido));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
