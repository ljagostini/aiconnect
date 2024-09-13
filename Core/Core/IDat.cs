namespace Percolore.Core
{
	public interface IDat
    {
        string CodigoCor { get; }
        string Quantidades { get; }

        bool Validar();
        Dictionary<int, double> GetQuantidades();
    }
}