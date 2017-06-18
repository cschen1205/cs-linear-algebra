# cs-linear-algebra
C# implementation of linear algebra and matrix manipulation. The cs-matrix library is writen in .NET Core

# Install

```bash
Install-Package cs-matrix
```

# Usage

### Create and update matrix

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

### Matrix Factorization and Inversion

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

<<<<<<< HEAD

The code below shows how to Cholesky decomposition

```cs

=======
The code below shows how to do symmetric matrix inversion using eigen vector decomposition:

```cs
>>>>>>> 54a732fc26f15cf888a7c97f8b4ffbb2a3f4cbea
double[][] Data = new double[][]{
        new double[] { 12, -51, 4},
        new double[] { 6, 167, -68},
        new double[] { -4, 24, -41}
    };
SparseMatrix A = new SparseMatrix(Data);
<<<<<<< HEAD
IMatrix Sigma, U, Vstar;
Cholesky.Factorize(A, out U, out Sigma, out Vstar);
```

The code below shows how to use Cholesky for matrix inversion

```cs

double[][] Data = new double[][]{
        new double[] { 12, -51, 4},
        new double[] { 6, 167, -68},
        new double[] { -4, 24, -41}
    };
SparseMatrix A = new SparseMatrix(Data);
IMatrix Ainv = CholeskySolver.Invert(A);
```

### Solving Linear System

The code below shows how to solve a set of equations in a linear system, A * x = b:


```cs
IVector x = new SparseVector(new double[] { 2, 4, 1 });
IVector b = A.Multiply(x);

IVector x_pi = QRSolver.Solve(A, b); // solve for x = b \ A using QR factorization
IVector x_pi = CholeskySolver.Solve(A, b); // solve for x = b \ A using Cholesky factorization
IVector x_pi = SVDSolver.Solve(A, b); // solve for x = b \ A using SVD factorization

```



