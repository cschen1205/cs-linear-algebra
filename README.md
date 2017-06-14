# cs-linear-algebra
C# implementation of linear algebra and matrix manipulation. The cs-matrix library is writen in .NET Core

# Install

```bash
Install-Package cs-matrix
```

# Usage

The code below shows how to create a matrix from a two-dimension array
```cs
double[][] Data = new double[][]{
        new double[] { 12, -51, 4},
        new double[] { 6, 167, -68},
        new double[] { -4, 24, -41}
    };
SparseMatrix A = new SparseMatrix(Data);
```

The code below shows how to create a sparse matrix with 3 rows and 3 columns and has cell(1, 1) = 1and other cells equal to 0


```cs

SparseMatrix A = new SparseMatrix(3, 3);
A[1, 1]  = 1;
```

The code below shows how to SVD decomposition

```cs

double[][] Data = new double[][]{
        new double[] { 12, -51, 4},
        new double[] { 6, 167, -68},
        new double[] { -4, 24, -41}
    };
SparseMatrix A = new SparseMatrix(Data);
IMatrix Sigma, U, Vstar;
SVD.Factorize(A, out U, out Sigma, out Vstar);
```

The code below shows how to use SVD for matrix inversion

```cs

double[][] Data = new double[][]{
        new double[] { 12, -51, 4},
        new double[] { 6, 167, -68},
        new double[] { -4, 24, -41}
    };
SparseMatrix A = new SparseMatrix(Data);
IMatrix Ainv = SVDSolver.Invert(A);
```

The code below shows how to QR decomposition

```cs

double[][] Data = new double[][]{
        new double[] { 12, -51, 4},
        new double[] { 6, 167, -68},
        new double[] { -4, 24, -41}
    };
SparseMatrix A = new SparseMatrix(Data);
IMatrix Sigma, U, Vstar;
QR.Factorize(A, out U, out Sigma, out Vstar);
```

The code below shows how to use QR for matrix inversion

```cs

double[][] Data = new double[][]{
        new double[] { 12, -51, 4},
        new double[] { 6, 167, -68},
        new double[] { -4, 24, -41}
    };
SparseMatrix A = new SparseMatrix(Data);
IMatrix Ainv = QRSolver.Invert(A);
```

