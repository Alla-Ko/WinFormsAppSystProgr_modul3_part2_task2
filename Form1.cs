using System.Diagnostics;

namespace WinFormsAppSystProgr_modul3_part2_task2_240520
{
    public partial class Form1 : Form
    {
        List<Horse> horses;
        List<Horse> horsesNew;
        List<Label> names;
        List<PictureBox> avatars;
        List<PictureBox> places;
        List<ProgressBar> progressBars;
        List<Task> tasks;
        List<Image> rewards;
        Random random = new Random();
        CancellationTokenSource cts;

        public Form1()
        {
            InitializeComponent();
            Icon = Res.icon;
            tasks = new List<Task>();
            horsesNew = new List<Horse>();
            horses = new List<Horse>
            {
                new Horse { Avatar = Res._0, Name = "Choco Puff" },
                new Horse { Avatar = Res._1, Name = "Lightning Bolt" },
                new Horse { Avatar = Res._2, Name = "Stardust Sprinkles" },
                new Horse { Avatar = Res._3, Name = "Fluffy Dumpling" },
                new Horse { Avatar = Res._4, Name = "Rainbow Dreamer" }
            };
            names = new List<Label>
            {
                label0,
                label1,
                label2,
                label3,
                label4
            };
            avatars = new List<PictureBox>
            {
                pictureBox1,
                pictureBox2,
                pictureBox3,
                pictureBox4,
                pictureBox5
            };
            places = new List<PictureBox>
            {
                pictureBox6,
                pictureBox7,
                pictureBox8,
                pictureBox9,
                pictureBox10
            };
            progressBars = new List<ProgressBar>
            {
                progressBar0,
                progressBar1,
                progressBar2,
                progressBar3,
                progressBar4
            };
            rewards = new List<Image>
            {
                Res._1place,
                Res._2place,
                Res._3place,
                Res._4place,
                Res._5place
            };

            StartSettings();

            // Додати обробник події закриття форми
            this.FormClosing += Form1_FormClosing;
        }

        private void StartSettings()
        {
            for (int i = 0; i < horses.Count; i++)
            {
                avatars[i].Image = horses[i].Avatar;
                names[i].Text = horses[i].Name;
                places[i].Image = rewards[i];

                progressBars[i].Value = 0; // Скидання значення прогресбару
                places[i].Visible = false;
                progressBars[i].Visible = false;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            StartSettings();
            tasks.Clear(); // Очищення списку завдань перед початком
            horsesNew.Clear(); // Очищення списку завершених коней перед початком

            // Ініціалізувати CancellationTokenSource
            cts = new CancellationTokenSource();
            button1.Enabled = false;
            for (int i = 0; i < progressBars.Count; i++)
            {
                progressBars[i].Visible = true;
                int index = i; // Локальна копія змінної i
                Task newTask = Task.Run(() =>
                {
                    Debug.WriteLine($"Horse {index} started running");
                    Run(horses[index], progressBars[index], cts.Token);
                    lock (horsesNew)
                    {
                        horsesNew.Add(horses[index]);
                    }
                }, cts.Token);
                tasks.Add(newTask);
            }
            try
            {
                await Task.WhenAll(tasks);
                Thread.Sleep(700);

                for (int i = 0; i < horsesNew.Count; i++)
                {
                    avatars[i].Image = horsesNew[i].Avatar;
                    names[i].Text = horsesNew[i].Name;
                    places[i].Visible = true;
                    progressBars[i].Visible = true;
                }
                button1.Enabled = true;
            }
            catch (OperationCanceledException)
            {
                // Обробка скасованих завдань
                Debug.WriteLine("Tasks were cancelled.");
            }
        }

        private void Run(Horse horse, ProgressBar progressBar, CancellationToken token)
        {
            for (int i = 0; i <= 100; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                if (!token.IsCancellationRequested)
                {
                    if (progressBar.InvokeRequired)
                    {
                        progressBar.Invoke(new Action(() =>
                        {
                            progressBar.Value = i;
                        }));
                    }
                    else
                    {
                        progressBar.Value = i;
                    }
                    Thread.Sleep(random.Next(50, 500));
                }
                else return;
            }
        }


        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel(); // Скасування всіх задач
                await Task.Delay(500); // Невелика затримка для завершення всіх задач
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                // Ігнорувати виключення скасування
            }
            catch (Exception ex)
            {
                // Інші можливі виключення
                Debug.WriteLine($"Exception: {ex.Message}");
            }

            // Очищення списків та звільнення ресурсів
            tasks.Clear();
            cts.Dispose();
        }
    }

    public class Horse
    {
        public Image Avatar { get; set; }
        public string Name { get; set; }
        public ProgressBar PprogressBar = new ProgressBar();
    }
}
