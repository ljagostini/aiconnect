namespace Percolore.Core
{
	public struct Fracao
    {
        public double Numerador { get; internal set; }
        public double Denominador { get; internal set; }

        #region Construtores

        public Fracao(double numerador, double denominador)
        {
            if (denominador == 0)
                throw new ArgumentException("O denominador não pode ser igul a a zero.");

            if (denominador < 0)
            {
                //Denominador negativo.
                //Isso não altera o valor global da fração, apenas a sua representação.
                numerador = -numerador;
                denominador = -denominador;
            }

            this.Numerador = numerador;
            this.Denominador = denominador;
        }

        public Fracao(double numerador, Fracao denominador)
        {
            //divide the numerador by the denominador fracao
            this = new Fracao(numerador, 1) / denominador;
        }

        public Fracao(Fracao numerador, double denominador)
        {
            //multiply the numerador fracao by 1 over the denominador
            this = numerador * new Fracao(1, denominador);
        }

        public Fracao(Fracao fracao)
        {
            this.Numerador = fracao.Numerador;
            this.Denominador = fracao.Denominador;
        }

        #endregion

        #region Métodos públicos

        public Fracao ToDenominador(double newDenominador)
        {
            Fracao newfracao = this;

            if (this.Denominador == newDenominador)
                return newfracao;

            //Novo denominador não pode ser menor que denominador atual
            if (newDenominador < this.Denominador)
                return newfracao;

            //Novo denominador deve ser divisível pelo atual denominador
            if (newDenominador % this.Denominador != 0)
                return newfracao;

            //Multiplica a fração pelo fator para encontrar numerador
            //equivalente ao novo denominador
            double factor = newDenominador / this.Denominador;
            newfracao.Denominador = newDenominador;
            newfracao.Numerador *= factor;

            return newfracao;
        }

        public Fracao GetReducao()
        {
            //Reduz a fração ao menor termo
            Fracao modifiedfracao = this;

            //While the numerador and denominador share a greatest common denominador,
            //keep dividing both by it
            double gcd = 0;
            while (Math.Abs(gcd = GetMDC(modifiedfracao.Numerador, modifiedfracao.Denominador)) != 1)
            {
                modifiedfracao.Numerador /= gcd;
                modifiedfracao.Denominador /= gcd;
            }

            //Make sure only a single negative sign is on the numerador
            if (modifiedfracao.Denominador < 0)
            {
                modifiedfracao.Numerador = -this.Numerador;
                modifiedfracao.Denominador = -this.Denominador;
            }

            return modifiedfracao;
        }

        public override string ToString()
        {
            return Numerador + "/" + Denominador;
        }

        public double ToDouble()
        {
            return this.Numerador / this.Denominador;
        }

        //public static implicit operator double(Fracao f)
        //{
        //    return f.Numerador / f.Denominador;
        //}

        #endregion

        #region Operadores

        public static Fracao operator +(Fracao fracao1, Fracao fracao2)
        {
            //Check if either fracao is zero
            if (fracao1.Denominador == 0)
                return fracao2;
            else if (fracao2.Denominador == 0)
                return fracao1;

            //Get Least Common Denominador
            double lcd = GetMenorTermo(fracao1.Denominador, fracao2.Denominador);

            //Transform the fracaos
            fracao1 = fracao1.ToDenominador(lcd);
            fracao2 = fracao2.ToDenominador(lcd);

            //Return sum
            return new Fracao(fracao1.Numerador + fracao2.Numerador, lcd).GetReducao();
        }

        public static Fracao operator -(Fracao fracao1, Fracao fracao2)
        {
            //Get Least Common Denominador
            double lcd = GetMenorTermo(fracao1.Denominador, fracao2.Denominador);

            //Transform the fracaos
            fracao1 = fracao1.ToDenominador(lcd);
            fracao2 = fracao2.ToDenominador(lcd);

            //Return difference
            return new Fracao(fracao1.Numerador - fracao2.Numerador, lcd).GetReducao();
        }

        public static Fracao operator *(Fracao fracao1, Fracao fracao2)
        {
            double numerador = fracao1.Numerador * fracao2.Numerador;
            double denomenator = fracao1.Denominador * fracao2.Denominador;

            return new Fracao(numerador, denomenator).GetReducao();
        }

        public static Fracao operator /(Fracao fracao1, Fracao fracao2)
        {
            return new Fracao(fracao1 * fracao2.GetReciprocal()).GetReducao();
        }

        #endregion

        #region Métodos privados

        private static double GetMDC(double a, double b)
        {
            //Drop negative signs
            a = Math.Abs(a);
            b = Math.Abs(b);

            //Return the greatest common denominador between two integers
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            if (a == 0)
                return b;
            else
                return a;
        }

        private static double GetMenorTermo(double a, double b)
        {
            //Return the Least Common Denominador between two integers
            return (a * b) / GetMDC(a, b);
        }

        private Fracao GetReciprocal()
        {
            //Inverte posições do numerador e denominador
            return new Fracao(this.Denominador, this.Numerador);
        }

        #endregion
    }
}