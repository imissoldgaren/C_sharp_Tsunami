using myLibrary;


[TestClass]
public class TestUnits
{
    [TestMethod]
    public void decomposeTest()
    {

         FWave fWave = new FWave();

        float[] i_alphas = {17,39};
        
        float[] i_eigens = {-9.73f,9.57f};
        float[] i_minus_A_deltaQ = new float[2];
        float[] i_plus_A_deltaQ =  new float[2];
        
        fWave.Decompose(i_alphas,
                        i_eigens,
                        out i_minus_A_deltaQ,
                        out i_plus_A_deltaQ );

        Assert.AreEqual(17,i_minus_A_deltaQ[0],0.0001f );
        Assert.AreEqual( -165.41 ,i_minus_A_deltaQ[1],0.0001f );

        Assert.AreEqual( 39 ,i_plus_A_deltaQ[0],0.0001f );
        Assert.AreEqual( 373.23 ,i_plus_A_deltaQ[1],0.0001f );

       
        
    }

    [TestMethod]
     public void NetupdateTest()
    {

         FWave fWave = new FWave();

    
        float[] l_netUpdatesL = new float[2];
        float[] l_netUpdatesR =  new float[2];
        
        fWave.NetUpdates( 2.8f,
                        9.6f,
                        41.9f,
                         37.7f,
                        0,
                        0,
                        out l_netUpdatesL,
                        out l_netUpdatesR );

        Assert.AreEqual(0, l_netUpdatesL[0], 0.0001f );
        Assert.AreEqual( 0 ,l_netUpdatesL[1] , 0.0001f );

        Assert.AreEqual( -4.199690818786621 ,l_netUpdatesR[0],0.0001f );
        Assert.AreEqual( -65.50415802001953 ,l_netUpdatesR[1],0.0001f  );

        
    }
    [TestMethod]
     public void NetupdateTest1()
    {

         FWave fWave = new FWave();

    
        float[] l_netUpdatesL = new float[2];
        float[] l_netUpdatesR =  new float[2];
        
        fWave.NetUpdates( 10f,
                            9f,
                            -30f,
                            27f,
                            0,
                            0,
                        out l_netUpdatesL,
                        out l_netUpdatesR );

        Assert.AreEqual(33.55900192260742,l_netUpdatesL[0],0.0001f);
        Assert.AreEqual( -326.5663146972656 ,l_netUpdatesL[1],0.0001f );

        Assert.AreEqual( 23.440996170043945 ,l_netUpdatesR[0],0.0001f );
        Assert.AreEqual( 224.4031219482422 ,l_netUpdatesR[1],0.0001f );

        
    }

    [TestMethod]
     public void YourTestMethod()
    {
        WavePropagation1d m_waveProp = new WavePropagation1d(100,false);
        // Set properties for the first loop
        for (uint l_ce = 0; l_ce < 50; l_ce++)
        {
            m_waveProp.setHeight(l_ce, 0, 10);
            m_waveProp.setMomentumX(l_ce, 0, 0);
        }

        // Set properties for the second loop
        for (uint l_ce = 50; l_ce < 100; l_ce++)
        {
            m_waveProp.setHeight(l_ce, 0, 8);
            m_waveProp.setMomentumX(l_ce, 0, 0);
        }

        // Set outflow boundary condition
        m_waveProp.setGhostColumn();

        // Perform a time step
        m_waveProp.timeStep(0.1f);
        m_waveProp.timeStep(0.1f);

        // Steady state checks
        for (uint l_ce = 0; l_ce < 49; l_ce++)
        {
            Assert.AreEqual(10, m_waveProp.getHeightValues()[l_ce], 0.0001f);
            Assert.AreEqual(0, m_waveProp.getMomentumXValues()[l_ce], 0.0001f);
        }

        // Dam-break checks
        Assert.AreEqual(9.127368927001953, m_waveProp.getHeightValues()[49]);
        Assert.AreEqual(8.021598815917969, m_waveProp.getMomentumXValues()[49]); //0 + 0.1 * 88.25985

        Assert.AreEqual(8.994317054748535, m_waveProp.getHeightValues()[50],0.0001f);
        Assert.AreEqual(9.19817066192627, m_waveProp.getMomentumXValues()[50],0.0001f);

        // More steady state checks
        /*for (uint l_ce = 51; l_ce < 100; l_ce++)
        {
            Assert.AreEqual(8.892641f, m_waveProp.getHeightValues()[l_ce], 0.0000000000000000000001f);
            Assert.AreEqual(9.496013641357422f, m_waveProp.getMomentumXValues()[l_ce],0.0000000000000000001f);
        }*/
    }
}
