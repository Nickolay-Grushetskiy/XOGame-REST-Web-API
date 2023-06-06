using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();



app.MapGet("/xo", () => 
{
    Data data = new Data();
    Load(ref data);
    return Results.Created("/", data);
});

app.MapPost("/xo", (Pos pos) =>
{
    int posLine = pos.x * 3 + pos.y;
    Data data = new Data();
    Load(ref data);
    if (data.field[posLine] != 'e')
    {
        data.status = 4;
        return Results.Created("/", data);
    }
    data.field[posLine] = data.player;
    data.count--;

    Check(ref data, pos);

    Save(data);
    return Results.Created("/", data);
});

app.MapDelete("/xo", () =>
{
    Data data = new Data();
    Save(data);
    return Results.Created("/", data);
});

app.Run();

void Check(ref Data data,Pos pos)
{
    // Создание временного поля 3x3
    char[,] tmpField = new char[3, 3];
    for (int i = 0, l = 0; i < 3; ++i)
    {
        for (int j = 0; j < 3; ++j, ++l)
        {
            tmpField[i, j] = data.field[l];
        }
    }
    bool win = true;
    // Проверка выигрыша по горизонтали
    for (int i = 0; i < 3 && win; ++i)
    {
        if (tmpField[pos.x, i] != data.player) win = false;
        if (i == 2 && win)
        {
            _ = data.player == 'x' ? data.status = 2 : data.status = 3;
            data.codeWin = pos.x + 1;
            return;
        }
    }
    win = true;
    // Проверка выигрыша по вертикали
    for (int i = 0; i < 3 && win; ++i)
    {
        if (tmpField[i, pos.y] != data.player) win = false;
        if (i == 2 && win)
        {
            _ = data.player == 'x' ? data.status = 2 : data.status = 3;
            data.codeWin = pos.y + 4;
            return;
        }
    }
    win = true;
    //Проверка выигрыша по диагонали \
    if (pos.x - pos.y == 0)
    {
        for (int i = 0; i < 3 && win; ++i)
        {
            if (tmpField[i, i] != data.player) win = false;
            if (i == 2 && win)
            {
                _ = data.player == 'x' ? data.status = 2 : data.status = 3;
                data.codeWin = 7;
                return;
            }
        }
    }
    //Проверка выигрыша по диагонали /
    win = true;
    for (int i = 0, j = 2; i < 3 && win; ++i, --j)
    {
        if (tmpField[i, j] != data.player) win = false;
        if (i == 2 && win)
        {
            _ = data.player == 'x' ? data.status = 2 : data.status = 3;
            data.codeWin = 8;
            return;
        }
    }
    // Проверка на ничью
    if (data.count == 0 && !win)
    {
        data.status = 1;
        return ;
    }
    _ = data.player == 'x' ? data.player = 'o' : data.player = 'x';
}

static void Save(Data data)
{
    var opt=new JsonSerializerOptions { WriteIndented = true };
    using FileStream fs = new FileStream("Data.json", FileMode.OpenOrCreate);
    JsonSerializer.SerializeAsync<Data>(fs, data, opt);
}

static void Load(ref Data data)
{
    using FileStream fs = new FileStream("Data.json", FileMode.Open);

    data = JsonSerializer.Deserialize<Data>(fs);

}
class Data
{
    public char[] field { get; set; }
    public char player { get; set; }
    public int status { get; set; }
    public int codeWin { get; set; }
    public int count { get; set; }
    public Data()
    {
        field = new char[] { 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e', 'e' };
        count = 9;
        status = 0;
        codeWin = 0;
        player = 'x';
    }
};

class Pos
{
    public int x { get; set; }
    public int y { get; set; }
}

