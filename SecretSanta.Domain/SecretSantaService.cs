using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SecretSanta.Domain
{
    public class SecretSantaService
    {
        public List<(Santa, Santa)> AssignSantas ([NotNull] Santa[] santas)
        {
            if (santas.Length < 2)
                throw new ArgumentException ("Must have at least to elements.", nameof(santas));
                
            var random = new Random();
            var santas2 = santas.ToList();

            var valueTuples = new List<(Santa, Santa)>();
            foreach (var santa in santas)
            {
                Santa santa2;
                int i;
                do
                {
                    i = random.Next() % santas2.Count;
                    santa2 = santas2[i];
                } while (santa == santa2);
                
                santas2.RemoveAt (i);
                valueTuples.Add (new(santa, santa2));
            }

            return valueTuples;
        }
    }
}