namespace Percolore.Treinamento
{
	public class Conteudo
    {
        public string Etapa { get; set; }
        public string Texto { get; set; }
        public List<Image> Imagens { get; }
      
        public Conteudo()
        {
            Imagens = new List<Image>();           
        }
    }
}