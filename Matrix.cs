public static class Matrix
{
    public static double[,] Mean3x3
    {
        get
        {
            return new double[,]
           { {  1, 1, 1, },
             {  1, 1, 1, },
             {  1, 1, 1, }, };
        }
    }
    public static double[,] Mean5x5
    {
        get
        {
            return new double[,]
           { {  1, 1, 1, 1, 1, },
             {  1, 1, 1, 1, 1,},
             {  1, 1, 1, 1, 1, },
             {  1, 1, 1, 1, 1, },
             {  1, 1, 1, 1, 1, }, };
        }
    }

    public static double[,] Perwittsx
    {
        get
        {
            return new double[,]
           { {  -1, -1, -1, },
             {  0, 0, 0, },
             {  1, 1, 1, }, };
        }
    }
    public static double[,] Perwittsy
    {
        get
        {
            return new double[,]
           { {  -1, 0, 1, },
             {  -1, 0, 1, },
             {  -1, 0, 1, }, };
        }
    }
    public static double[,] Sorbelx
    {
        get
        {
            return new double[,]
           { {  -1, 0, 1, },
             {  -2, 0, 2, },
             {  -1, 0, 1, }, };
        }
    }
    public static double[,] Sorbely
    {
        get
        {
            return new double[,]
           { {  -1, -2, -1, },
             {  0, 0, 0, },
             {  1, 2, 1, }, };
        }
    }
    public static double[,] Laplacian
    {
        get
        {
            return new double[,]
           { {  -1, -1, -1, },
             {  -1, 8, -1, },
             {  -1, -1, -1, }, };
        }
    }
    public static double[,] Gaussian3x3
    {
        get
        {
            return new double[,]
           { {  1, 2, 1, },
             {  2, 4, 2, },
             {  1, 2, 1, }, };
        }
    }
    public static double[,] Gaussian5x5
    {
        get
        {
            return new double[,]
           { {  1, 4, 7, 4, 1 },
             {  4, 16, 26, 16, 4 },
             {  7, 26, 41, 26, 7 },
             {  4, 16, 26, 16, 4 },
             {  1, 4, 7, 4, 1 },};
        }
    }
}