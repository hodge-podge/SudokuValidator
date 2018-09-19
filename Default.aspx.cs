using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
/*
 *  SudoKu Validator
 *  Written By: Bruce Hodge
 *  9/10/2018
 * 
 */
public partial class _Default : System.Web.UI.Page
{
    static bool debug = false;
    static string sudoku;//holds the rows of digits
    static char tilda = '~';//row separator 
    static int[,] sudokuArray = new int[9, 9];//internal Array used for validating
    static int rowIdx = 0;//keeps track of current row index
    Block[] blocks = new Block[9] { new Block(), new Block(), new Block(), new Block(), new Block(), new Block(), new Block(), new Block(), new Block() };
    //track blocks
    int[] good = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };//reference for good block & valid digits

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
        {
            sudoku = RowData.Value;
            showTable();
        }
        else
        {
            sudoku = "";
            LabelMessage.Attributes["class"] = "error";
        }
    }

    protected void addRow(string row)
    {
        string[] nums = row.Trim(' ').Split(' ');
        TableRow tRow = new TableRow();
        if (nums.Length == 9)
        {
            for (int i = 1; i <= 9; i++)
            {
                TableCell tCell = new TableCell();
                tCell.Text = nums[i - 1];
                AddCssClass(tCell, "col" + i);
                if (!Int32.TryParse(nums[i - 1], out sudokuArray[rowIdx, i - 1])) sudokuArray[rowIdx, i - 1] = 0;
                if ("123456789".IndexOf(nums[i - 1]) == -1)
                {
                    AddCssClass(tCell, "invalidEntry");
                    tCell.Attributes["title"] = "Invalid Entry!";
                }
                tRow.Controls.Add(tCell);
            }
            rowIdx++;
            TableSudoKu.Rows.Add(tRow);
        }
    }

    protected void showTable()
    {
        rowIdx = 0;
        string[] rows = sudoku.Split(tilda);
        foreach (string row in rows)
        {
            addRow(row);
        }
    }

    protected void ButtonSubmit_Click(object sender, EventArgs e)
    {
        int cnt = 0;
        LabelMessage.Attributes["class"] = "error";
        foreach (string s in TextBoxInput.Text.Trim(' ').Split(' '))
        {
            try
            {
                cnt++;
                int d = short.Parse(s);
                //if (d < 1 || d > 9) //Trap invalid digits from being entered <- this would prevent zeros from even being allow in
                if (d < 0 || d > 9)
                {
                    LabelMessage.Text = string.Format("Invalid number@[{0}] in sequence : {1}", cnt, s);
                    return;
                }
            }
            catch (FormatException ex)
            {
                LabelMessage.Text = string.Format("@[{0}] {1}", cnt, ex.Message);
                return;
            }
        };
        if (cnt != 9)
        {
            LabelMessage.Text = "9 single digits were not supplied!";
        }
        else
        {
            if (sudoku.Split(tilda).Length <= 9)
            {
                LabelMessage.Text = "";
                addRow(TextBoxInput.Text);
                sudoku += TextBoxInput.Text + tilda;
                TextBoxInput.Text = "";
                RowData.Value = sudoku;
                if (sudoku.Split(tilda).Length > 9) ButtonValidate.Visible = true;
            }
            else
            {
                TextBoxInput.Text = "";
                ButtonValidate.Visible = true;
            }
        }
    }

    protected int[] scan(int[,] array2Scan)
    {
        int[] bad = new int[9];

        for (int i = 0; i < 9; i++)
        {
            int sum = 0;
            bool ok = true;
            bad[i] = 0;//assume good
            Dictionary<int, int> cell = new Dictionary<int, int>();
            for (int j = 0; j < 9; j++)
            {
                sum += array2Scan[i, j];
                if (cell.Keys.Contains(array2Scan[i, j])) ok = false;//found duplicate
                if (!good.Contains(array2Scan[i, j])) ok = false;//invalid digit like Zero!
                cell[array2Scan[i, j]] = 1;
                if (i >= 0 && i < 3)
                {
                    if (j >= 0 && j < 3)
                    {
                        blocks[0].Add(i, j, array2Scan[i, j]);
                    }
                    else if (j >= 3 && j < 6)
                    {
                        blocks[1].Add(i, j, array2Scan[i, j]);
                    }
                    else
                    {
                        blocks[2].Add(i, j, array2Scan[i, j]);
                    }
                }
                else if (i >= 3 && i < 6)
                {
                    if (j >= 0 && j < 3)
                    {
                        blocks[3].Add(i, j, array2Scan[i, j]);
                    }
                    else if (j >= 3 && j < 6)
                    {
                        blocks[4].Add(i, j, array2Scan[i, j]);
                    }
                    else
                    {
                        blocks[5].Add(i, j, array2Scan[i, j]);
                    }
                }
                else
                {
                    if (j >= 0 && j < 3)
                    {
                        blocks[6].Add(i, j, array2Scan[i, j]);
                    }
                    else if (j >= 3 && j < 6)
                    {
                        blocks[7].Add(i, j, array2Scan[i, j]);
                    }
                    else
                    {
                        blocks[8].Add(i, j, array2Scan[i, j]);
                    }
                }
            }
            if (sum != 45 || !ok) bad[i] = 1;
        }

        return bad;
    }

    protected void ButtonValidate_Click(object sender, EventArgs e)
    {
        bool ok = true;
        int[] notOk = scan(sudokuArray);//rows first
        for (int i = 0; i < 9; i++)
        {
            if (notOk[i] == 1)
            {
                ok = false;
                AddCssClass(TableSudoKu.Rows[i], "invalidRow");
                for (int j = 0; j < 9; j++) TableSudoKu.Rows[i].Cells[j].Attributes["title"] = "Row is Invalid";
            }

            blocks[i].data.Sort();//now check blocks by sorting them and comparing them to good block reference
            if (!Enumerable.SequenceEqual(blocks[i].data.Cast<int>().ToArray(), good))
            {
                for (int j = 0; j < 9; j++)
                {
                    TableCell currentCell = TableSudoKu.Rows[blocks[i].row[j]].Cells[blocks[i].col[j]];//get current cell of pairs
                    AddCssClass(currentCell, "strike");//strike out digits in block
                }
            }
        }
        notOk = scan(MatrixTranspose(sudokuArray));//columns next by Transposing matrix
        for (int i = 0; i < 9; i++)
        {
            if (notOk[i] == 1)
            {
                ok = false;
                for (int j = 0; j < 9; j++)
                {
                    TableCell currentCell = TableSudoKu.Rows[j].Cells[i];
                    if (currentCell.Attributes["title"] != null)
                    {
                        if (currentCell.Attributes["title"] != "Invalid entry!")
                        {
                            currentCell.Attributes["title"] += " and Column is Invalid!";
                            RemoveCssClass(currentCell, "invalidRow");
                            AddCssClass(currentCell, "invalidRowCol");
                        }
                    }
                    else
                    {
                        currentCell.Attributes["title"] = "Column is invalid!";
                        AddCssClass(currentCell, "invalidCol");
                    }
                }
            }
        }
        if (ok)
        {
            LabelMessage.Attributes["class"] = "validated";
            LabelMessage.Text = "Validated!";
            ButtonValidate.Visible = false;
            ButtonDone.Visible = true;
        }
        else
        {
            LabelMessage.Attributes["class"] = "error";
            LabelMessage.Text = "Invalid! StrikeThrough=Bad Block";
            ButtonValidate.Visible = false;
            ButtonDone.Visible = true;
        }
    }
    protected int[,] MatrixTranspose(int[,] matrix)
    {
        int[,] transposed = new int[9, 9];
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                transposed[row, column] = matrix[column, row];
            }
        }

        return transposed;
    }

    protected void ButtonDone_Click(object sender, EventArgs e)
    {
        resetApp();
    }

    protected void resetApp()
    {
        LabelMessage.Attributes["class"] = "error";
        LabelMessage.Text = "";
        sudoku = "";//clear out buffer
        RowData.Value = sudoku;//clear hidden field on form
        sudokuArray = new int[9, 9];//clear out array
        rowIdx = 0;//init to first row
        ButtonDone.Visible = false;
        ButtonValidate.Visible = false;
        TableSudoKu.Rows.Clear();
    }

    /* Helper/Debug functions..... */

    protected void RemoveCssClass(WebControl control, String css)
    {
        control.CssClass = String.Join(" ", control.CssClass.Split(' ').Where(x => x != css).ToArray());
    }

    protected void AddCssClass(WebControl control, String css)
    {
        RemoveCssClass(control, css);
        css += " " + control.CssClass;
        control.CssClass = css;
    }

    protected void inputMatrix(int[,] matrix)
    {
        resetApp();
        var rows = Enumerable.Range(0, matrix.GetLength(0))
           .Select(row => Enumerable.Range(0, matrix.GetLength(1))
           .Select(col => matrix[row, col]).Select(x => x.ToString()).ToArray())
           .ToList();

        for (int i = 0; i < 9; i++)
        {
            TextBoxInput.Text = String.Join(" ", rows[i]);
            ButtonSubmit_Click(ButtonSubmit, EventArgs.Empty);
        }

    }

    protected void ButtonDebugValid_Click(object sender, EventArgs e)
    {
        int[,] matrix = new int[,] {
        {5,3,4,6,7,8,9,1,2},
        {6,7,2,1,9,5,3,4,8},
        {1,9,8,3,4,2,5,6,7},
        {8,5,9,7,6,1,4,2,3},
        {4,2,6,8,5,3,7,9,1},
        {7,1,3,9,2,4,8,5,6},
        {9,6,1,5,3,7,2,8,4},
        {2,8,7,4,1,9,6,3,5},
        {3,4,5,2,8,6,1,7,9}
        };
        inputMatrix(matrix);
    }

    protected void ButtonDebugInValid_Click(object sender, EventArgs e)
    {
        int[,] matrix = new int[,] {
        {5,3,4,6,7,8,9,1,2},
        {6,7,2,1,9,0,3,4,8},
        {1,0,0,3,4,2,5,6,0},
        {8,5,9,7,6,1,0,2,0},
        {4,2,6,8,5,3,7,9,1},
        {7,1,3,9,2,4,8,5,6},
        {9,0,1,5,3,7,2,1,4},
        {2,8,7,4,1,9,6,3,5},
        {3,0,0,4,8,1,1,7,9}
        };
        inputMatrix(matrix);
    }

    protected void ButtonDebugBroken_Click(object sender, EventArgs e)
    {
        int[,] matrix = new int[,] {
        {5,3,4,6,7,8,9,1,2},
        {6,7,2,1,9,5,3,4,8},
        {1,9,8,1,4,2,5,6,7},
        {8,5,9,7,6,1,4,2,3},
        {4,2,6,8,5,3,7,9,1},
        {7,1,3,9,2,4,8,5,6},
        {9,6,1,5,3,7,2,8,4},
        {2,8,7,4,1,9,6,3,5},
        {3,4,5,2,8,6,1,7,9}
        };
        inputMatrix(matrix);
    }

    protected void ButtonDebug_Click(object sender, EventArgs e)
    {
        Button bd = (Button)sender;
        debug ^= true;
        ButtonDebugValid.Visible = debug ? true : false;
        ButtonDebugInValid.Visible = debug ? true : false;
        ButtonDebugBroken.Visible = debug ? true : false;
    }
}

class Block
{
    public List<int> row = new List<int>();
    public List<int> col = new List<int>();
    public List<int> data = new List<int>();

    public void Add(int r, int c, int d)
    {
        this.row.Add(r);
        this.col.Add(c);
        this.data.Add(d);
    }
}
