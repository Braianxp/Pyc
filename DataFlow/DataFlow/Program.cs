using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();

            int cantidad = 10;

            int[] valores = new int[cantidad];
            //Generando valores aleatorios
            for (int i = 0; i < valores.Length; i++)
            {
                valores[i] = random.Next(0, 100);
            }

            BlockingCollection<int> inputQueue = new BlockingCollection<int>(100);
            //Colocando valores en cola principal
            for (int i = 0; i < valores.Length; i++)
            {
                inputQueue.Add(valores[i]);
            }

            BlockingCollection<int> ParesQueue = new BlockingCollection<int>(10);

            BlockingCollection<int> ImparesQueue = new BlockingCollection<int>(10);

            BlockingCollection<String> outQueue = new BlockingCollection<String>(30);

            //Productor
            Task T = Task.Factory.StartNew(() => {
                foreach (int valor in inputQueue.GetConsumingEnumerable())
                {

                    if (0 == valor % 2)
                    {
                        Console.WriteLine("Se agrego valor " + valor + " cola par");
                        ParesQueue.Add(valor);
                    }
                    else if (0 != valor % 2)
                    {
                        Console.WriteLine("Se agrego valor " + valor + " cola impar");
                        ImparesQueue.Add(valor);
                    }
                }
            });
            //consumidor
            Task TPares = Task.Factory.StartNew(() =>
            {
                foreach (int valor in ParesQueue.GetConsumingEnumerable())
                {
                    String texto = "";
                    texto = "Respondio TareaPar - valor = " + valor;

                    outQueue.Add(texto);
                }
            });
            //consumidor
            Task TImpar = Task.Factory.StartNew(() =>
            {
                foreach (int valor in ImparesQueue.GetConsumingEnumerable())
                {
                    String texto = "";
                    texto = "Respondio TareaImpar - valor = " + valor;

                    outQueue.Add(texto);
                }
            });

            //Final
            Task TFinal = Task.Factory.StartNew(() =>
            {

                foreach (string texto in outQueue.GetConsumingEnumerable())
                {
                    Console.WriteLine(texto);
                }
            });

            T.Wait();

            ImparesQueue.CompleteAdding();

            ParesQueue.CompleteAdding();

            Task.WaitAll(TImpar, TPares);

            outQueue.CompleteAdding();

            TFinal.Wait();
            Console.ReadLine();
        }
    }
}
