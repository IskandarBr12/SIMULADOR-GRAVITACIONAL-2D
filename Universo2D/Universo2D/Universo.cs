using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Universo
{
    class Universo
    {   // Força -> medida em N
        // Massa -> medida em Kg
        // Distância -> medida em m
        // G = 6,67408 X 10E-11 

        private ObservableCollection<Corpos> listaCorp;
        private double G = 6.67408 * Math.Pow(10, -11.0);

        public Universo()
        {
            listaCorp = new ObservableCollection<Corpos>();
        }

        public Corpos getCorpo(int pos)
        {
            if ((pos >= 0) && (pos < listaCorp.Count()))
            {
                return listaCorp.ElementAt(pos);
            }
            else
            {
                return null;
            }
        }

        public ObservableCollection<Corpos> getCorpo()
        {
            return listaCorp;
        }

        public void setCorpo(Corpos cp, int pos){
            if(pos < listaCorp.Count())
            {
                listaCorp.ElementAt(pos).copCorp(cp);
            }
            else 
            {
                listaCorp.Add(cp);
            }
        }

        public int qtdCorp() {
            return listaCorp.Count();
        }

        public double dist(Corpos c1, Corpos c2)
        {
            double b, c;

            b = c1.getPosY() - c2.getPosY();
            c = c1.getPosX() - c2.getPosX();

            return Math.Sqrt(Math.Pow(b, 2) + Math.Pow(c, 2));
        }

        private void forcaG(Corpos c1, Corpos c2)
        {
            double hipotenusa = dist(c2, c1);
            if (hipotenusa == 0) return;

            double catetoAdjacenteC1 = c2.getPosY() - c1.getPosY();
            double catetoOpostoC1 = c2.getPosX() - c1.getPosX();

            double forca = G * ((c1.getMassa() * c2.getMassa()) / Math.Pow(hipotenusa, 2));
            double forcaY = catetoAdjacenteC1 * forca / hipotenusa;
            double forcaX = catetoOpostoC1 * forca / hipotenusa;

            lock (c1)
            {
                c1.setForcaX(c1.getForcaX() + forcaX);
                c1.setForcaY(c1.getForcaY() + forcaY);
            }

            lock (c2)
            {
                c2.setForcaX(c2.getForcaX() - forcaX);
                c2.setForcaY(c2.getForcaY() - forcaY);
            }
        }


        private bool colisao(Corpos c1, Corpos c2)
        {
            double Px;
            double Py;
            double d;
            bool teveColisao = false;

            if ((dist(c1, c2)) <= (c1.getRaio() + c2.getRaio()))
            {
                teveColisao = true;
                Px = (c1.getMassa() * c1.getVelX()) + (c2.getMassa() * c2.getVelX());
                Py = (c1.getMassa() * c1.getVelY()) + (c2.getMassa() * c2.getVelY());

                d = ((c1.getMassa() * c1.getDensidade() + c2.getMassa() * c2.getDensidade()) / 
                     (c1.getMassa() + c2.getMassa()));

                if (c1.getMassa() >= c2.getMassa())
                {
                    c1.setNome(c1.getNome() + c2.getNome());
                    c1.setMassa(c1.getMassa() + c2.getMassa());
                    c1.setDensidade(d);
                    
                    c1.setVelX(Px / c1.getMassa());
                    c1.setVelY(Py / c1.getMassa());

                    c2.setValido(false);
                }
                else
                {
                    c2.setNome(c2.getNome() + c1.getNome());
                    c2.setMassa(c2.getMassa() + c1.getMassa());
                    c2.setDensidade(d);

                    c2.setVelX(Px / c2.getMassa());
                    c2.setVelY(Py / c2.getMassa());

                    c1.setValido(false);

                }
            }
            return teveColisao;
        }

        public void carCorp(int numCorpos, int xIni, int xFim, int yIni, int yFim, int masIni, int masFim)
        {
            int i;
            string nome;
            int massa;

            Random rd = new Random();

            for (i = 0; i < numCorpos; i++)
            {
                nome = "cp" + i;
                massa = rd.Next(masIni, masFim);
                listaCorp.Add(new Corpos(nome, massa,
                                              rd.Next(xIni, xFim), rd.Next(yIni, yFim), 0,
                                              0, 0, 0,rd.Next(1,255)));
            }
        }

        public void interCorp(int qtdSegundos)
        {
            bool teveColisao = false;
            zeraForc();

            Parallel.For(0, qtdCorp() - 1, i =>
            {
                for (int j = i + 1; j < qtdCorp(); j++)
                {
                    if (colisao(listaCorp[i], listaCorp[j]))
                    {
                        teveColisao = true;
                    }
                }
            });

            if (teveColisao)
            {
                OrganizaUniverso();
            }

            Parallel.For(0, qtdCorp() - 1, i =>
            {
                for (int j = i + 1; j < qtdCorp(); j++)
                {
                    forcaG(listaCorp[i], listaCorp[j]);
                }
            });

            Parallel.For(0, qtdCorp(), i =>
            {
                calcVelPosCorpos(qtdSegundos, listaCorp[i]);
            });
        }

        public void intCorp(int qtdInt, int qtdSegundos)
        {

            while (qtdInt > 0)
            {
                bool Colisao = false;
                zeraForc();
                int i = 0;

                for (i = 0; i < qtdCorp() - 1; i++)
                {
                    for (int j = i + 1; j < qtdCorp(); j++)
                    {
                        forcaG(listaCorp[i], listaCorp[j]);
                    }

                    calcVelPosCorpos(qtdSegundos, listaCorp[i]);
                }
                calcVelPosCorpos(qtdSegundos, listaCorp[i]);

                for (i = 0; i < qtdCorp() - 1; i++)
                {
                    for (int j = i + 1; j < qtdCorp(); j++)
                    {
                        if(colisao(listaCorp[i], listaCorp[j]))
                        {
                            Colisao = true;
                        }
                    }
                }

                if (Colisao)
                {
                    OrganizaUniverso();
                }

                qtdInt--;
            } 
        }

        public void copiaUniverso(Universo u)
        {
            listaCorp = new ObservableCollection<Corpos>();
            Corpos cp;
            for (int i = 0; i < u.qtdCorp(); i++)
            {
                cp = new Corpos(u.getCorpo(i).getNome(),
                               u.getCorpo(i).getMassa(),
                               u.getCorpo(i).getPosX(),
                               u.getCorpo(i).getPosY(),
                               u.getCorpo(i).getPosZ(),
                               u.getCorpo(i).getVelX(),
                               u.getCorpo(i).getVelY(),
                               u.getCorpo(i).getVelZ(),
                               u.getCorpo(i).getDensidade());
                this.setCorpo(cp, i);
            }
        }

        private void zeraForc()
        {
            Parallel.For(0, qtdCorp(), i =>
            {
                listaCorp[i].setForcaX(0);
                listaCorp[i].setForcaY(0);
                listaCorp[i].setForcaZ(0);
            });
        }

        private void calcVelPosCorpos(int qtdSegundos, Corpos c1)
        {
            double acelX;
            double acelY;

            acelX = c1.getForcaX() / c1.getMassa();
            acelY = c1.getForcaY() / c1.getMassa();

            c1.setPosX(c1.getPosX() + (c1.getVelX() * qtdSegundos) + (acelX * Math.Pow(qtdSegundos, 2) / 2));
            c1.setVelX(c1.getVelX() + (acelX * qtdSegundos));

            c1.setPosY(c1.getPosY() + (c1.getVelY() * qtdSegundos) + (acelY * Math.Pow(qtdSegundos, 2) / 2));
            c1.setVelY(c1.getVelY() + (acelY * qtdSegundos));

        }

        
        private void OrganizaUniverso()
        {
            int i;
            for (i = 0; i < qtdCorp();  i++)
            {
                if(!listaCorp.ElementAt(i).getVal())
                {
                    listaCorp.RemoveAt(i);
                }
            }
        }
    }
}
