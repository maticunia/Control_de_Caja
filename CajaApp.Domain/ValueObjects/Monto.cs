namespace CajaApp.Domain.ValueObjects;

public record Monto
{
    public decimal Valor { get; }

    private Monto(decimal valor)
    {
        if (valor < 0)
            throw new ArgumentException("El monto no puede ser negativo", nameof(valor));
        
        Valor = valor;
    }

    public static Monto Create(decimal valor) => new(valor);

    public static implicit operator decimal(Monto monto) => monto.Valor;

    public static implicit operator Monto(decimal valor) => Create(valor);

    public override string ToString() => Valor.ToString("C");
}
