




using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using myLibrary;


namespace myLibrary
{
    public class WavePropagation2d : IWavePropagation
    {

        private FWave fWave = new FWave();
        private bool m_choiceBoundary;
        private uint m_xCells = 0;
        private uint m_yCells = 0;
        private float[] m_h;
        private float[] m_hu;
        private float[] m_hv;
        private float[] m_b;
        private float[] m_h_temp;
        private float[] m_momentum_temp;

   


        public uint GetIndex(uint i_ix, uint i_iy)
        {
            return (m_xCells + 2) * i_iy + i_ix;
        }




        public WavePropagation2d(uint i_xCells, uint i_yCells, bool i_choiceBoundary)
        {
            
            m_choiceBoundary = i_choiceBoundary;
            m_xCells = i_xCells;
            m_yCells = i_yCells;

            // allocate memory including a single ghost cell on each side
            m_h = new float[(m_xCells + 2) * (m_yCells + 2)];
            m_hu = new float[(m_xCells + 2) * (m_yCells + 2)];
            m_hv = new float[(m_xCells + 2) * (m_yCells + 2)];
            m_b = new float[(m_xCells + 2) * (m_yCells + 2)];
            m_h_temp = new float[(m_xCells + 2) * (m_yCells + 2)];
            m_momentum_temp = new float[(m_xCells + 2) * (m_yCells + 2)];
        }


        public void timeStep(float i_scaling)
        {
            

            for (uint l_ce = 0; l_ce < ((m_xCells + 2) * (m_yCells + 2)); l_ce++)
            {
                m_h_temp[l_ce] = m_h[l_ce];
                m_momentum_temp[l_ce] = m_hu[l_ce];
            }

          

            for (uint l_ey = 1; l_ey < m_yCells+1 ; l_ey++)
            {
                for (uint l_ex = 1; l_ex < m_xCells+1 ; l_ex++)
                {

                    float[][] l_netUpdates = new float[2][];
                    l_netUpdates[0] = new float[2];
                    l_netUpdates[1] = new float[2];

                    uint l_ceL = GetIndex(l_ex, l_ey );
                    uint l_ceR = GetIndex(l_ex+1 ,l_ey);

                    fWave.NetUpdates(m_h_temp[l_ceL],
                                    m_h_temp[l_ceR],
                                    m_momentum_temp[l_ceL],
                                    m_momentum_temp[l_ceR],
                                    m_b[l_ceL],
                                    m_b[l_ceR],
                                     out l_netUpdates[0],
                                     out l_netUpdates[1]);
                    //Console.WriteLine("m_h[10246] = " + m_h[10246] + "l_ex : " + l_ex + " " + " l_ey: " + l_ey );
                    m_h[l_ceL] -= i_scaling * l_netUpdates[0][0];
                    m_hu[l_ceL] -= i_scaling * l_netUpdates[0][1];
                    m_h[l_ceR] -= i_scaling * l_netUpdates[1][0];
                    m_hu[l_ceR] -= i_scaling * l_netUpdates[1][1];
            
                   // Console.WriteLine("m_h[10246] = " + m_h[10246] + "l_ex : " + l_ex + " " + " l_ey: " + l_ey );
                }

            }
            setGhostColumn();

            //m_h.CopyTo(m_h_temp,0);
            //m_hu.CopyTo(m_momentum_temp,0);

            for (uint l_ce = 0; l_ce < ((m_xCells + 2) * (m_yCells + 2)); l_ce++)
            {
                m_h_temp[l_ce] = m_h[l_ce];
                m_momentum_temp[l_ce] = m_hu[l_ce];
            }

            for (uint l_ex = 1; l_ex < m_xCells+1 ; l_ex++)
            {
                for (uint l_ey = 1; l_ey < m_yCells+1 ; l_ey++)
                {
                    float[][] l_netUpdates = new float[2][];
                    uint l_ceL = GetIndex(l_ex, l_ey);
                    uint l_ceR = GetIndex(l_ex, l_ey+1 );

                   fWave.NetUpdates(m_h_temp[l_ceL],
                                             m_h_temp[l_ceR],
                                             m_momentum_temp[l_ceL],
                                             m_momentum_temp[l_ceR],
                                             m_b[l_ceL],
                                             m_b[l_ceR],
                                              out l_netUpdates[0],
                                              out l_netUpdates[1]);

                    m_h[l_ceL] -= i_scaling * l_netUpdates[0][0];
                    m_hu[l_ceL] -= i_scaling * l_netUpdates[0][1];
                    m_h[l_ceR] -= i_scaling * l_netUpdates[1][0];
                    m_hu[l_ceR] -= i_scaling * l_netUpdates[1][1];
                }
            }
            setGhostRow();
            //Console.WriteLine("m_h[10246] = " + m_h[10246]);

            
        }

        private void setGhostRow()
        {

            if (m_choiceBoundary)
            {
                for (uint l_g = 1; l_g < m_xCells + 1; l_g++)
                {
                    m_h[l_g] = m_h[GetIndex(l_g, 1)];
                    m_h[GetIndex(l_g, m_yCells + 1)] = m_h[GetIndex(l_g, m_yCells)];
                    m_hv[l_g] = -m_hv[GetIndex(l_g, 1)];
                    m_hv[GetIndex(l_g, m_yCells + 1)] = -m_hv[GetIndex(l_g, m_yCells)];
                    m_b[l_g] = m_b[GetIndex(l_g, 1)];
                    m_b[GetIndex(l_g, m_yCells + 1)] = m_b[GetIndex(l_g, m_yCells)];
                }
            }
            else
            {
                for (uint l_g = 1; l_g < m_xCells + 1; l_g++)
                {
                    m_h[l_g] = m_h[GetIndex(l_g, 1)];
                    m_h[GetIndex(l_g, m_yCells + 1)] = m_h[GetIndex(l_g, m_yCells)];
                    m_hv[l_g] = m_hv[GetIndex(l_g, 1)];
                    m_hv[GetIndex(l_g, m_yCells + 1)] = m_hv[GetIndex(l_g, m_yCells)];
                    m_b[l_g] = m_b[GetIndex(l_g, 1)];
                    m_b[GetIndex(l_g, m_yCells + 1)] = m_b[GetIndex(l_g, m_yCells)];
                }
            }
        }

        public void setGhostColumn()
        {
            // leftmost and rightmost column
            if (m_choiceBoundary)
            {
                for (uint l_g = 1; l_g < m_yCells + 1; l_g++)
                {
                    m_h[GetIndex(0, l_g)] = m_h[GetIndex(1, l_g)];
                    m_h[GetIndex(m_xCells + 1, l_g)] = m_h[GetIndex(m_xCells, l_g)];
                    m_hu[GetIndex(0, l_g)] = -m_hu[GetIndex(1, l_g)];
                    m_hu[GetIndex(m_xCells + 1, l_g)] = -m_hu[GetIndex(m_xCells, l_g)];
                    m_b[GetIndex(0, l_g)] = m_b[GetIndex(1, l_g)];
                    m_b[GetIndex(m_xCells + 1, l_g)] = m_b[GetIndex(m_xCells, l_g)];
                }
            }
            else
            {
                for (uint l_g = 1; l_g < m_yCells + 1; l_g++)
                {
                    m_h[GetIndex(0, l_g)] = m_h[GetIndex(1, l_g)];
                    m_h[GetIndex(m_xCells + 1, l_g)] = m_h[GetIndex(m_xCells, l_g)];
                    m_hu[GetIndex(0, l_g)] = m_hu[GetIndex(1, l_g)];
                    m_hu[GetIndex(m_xCells + 1, l_g)] = m_hu[GetIndex(m_xCells, l_g)];
                    m_b[GetIndex(0, l_g)] = m_b[GetIndex(1, l_g)];
                    m_b[GetIndex(m_xCells + 1, l_g)] = m_b[GetIndex(m_xCells, l_g)];
                }
            }
        }




        public uint GetStride()
        {
            return m_xCells+2;
        }

        public float getHeight(uint i_x, uint i_y)
        {
            return m_h[GetIndex(i_x,i_y)];
        }

        public float getMomentumX(uint i_x, uint i_y)
        {
            return m_hu[GetIndex(i_x,i_y)];
        }

        public float getMomentumY(uint i_x, uint i_y)
        {
            return m_hv[GetIndex(i_x,i_y)];
        }

        public float getBathymetry(uint i_x, uint i_y)
        {
            return m_b[GetIndex(i_x,i_y)];
        }

        public uint getGhostcellX(uint i_x, uint i_y)
        {
            return 1;
        }

        public uint getGhostcellY(uint i_x, uint i_y)
        {
            return 1;
        }

        public void setHeight(uint i_x, uint i_y, float i_value)
        {
            m_h[GetIndex(i_x,i_y)] = i_value;
        }

        public void setBathymetry(uint i_x, uint i_y, float i_value)
        {
            m_b[GetIndex(i_x,i_y)] = i_value;
        }

        public void setMomentumX(uint i_x, uint i_y, float i_value)
        {
             m_hu[GetIndex(i_x,i_y )] = i_value;
        }

        public void setMomentumY(uint i_x, uint i_y, float i_value)
        {
            m_hv[GetIndex(i_x ,i_y)] = i_value;
        }

        public float[] getHeightValues()
        {
            return m_h;
        }

        public float[] getBathymetryValues()
        {
            return m_b;
        }

        public float[] getMomentumXValues()
        {
            return m_hu;
        }

        public float[] getMomentumYValues()
        {
            return m_hv;
        }
    }
}