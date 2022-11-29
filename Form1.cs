using Lucene.Net.Index;
using Lucene.Net.Store;
using System.Windows.Forms;

namespace Luke
{
    public partial class Form1 : Form
    {
        private FSDirectory _directory;
        private DirectoryReader _reader;

        public Form1()
        {
            InitializeComponent();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenIndex();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            CloseIndex();
        }

        private void OpenIndex()
        {
            var path = pathTextBox.Text;

            _directory = FSDirectory.Open(path);
            _reader = DirectoryReader.Open(_directory);

            bool firstTime = true;

            for (int i = 0; i < _reader.MaxDoc; i++)
            {
                var document = _reader.Document(i);

                if (firstTime)
                {
                    listView.Columns.Add(new ColumnHeader() { Text = "Document Id" });

                    foreach (var field in document.Fields)
                    {
                        listView.Columns.Add(new ColumnHeader() { Text = field.Name });
                    }

                    firstTime = false;
                }

                var items = new List<string>();
                items.Add(i.ToString());

                foreach (var field in document.Fields)
                {
                    if (field.Name == "date")
                    {
                        var date = document.GetField(field.Name).GetInt64Value().Value;
                        var dateTime = DateTime.FromFileTimeUtc(date);
                        items.Add($"{dateTime} ({date})");
                    }
                    else
                    {
                        var x = document.Get(field.Name);
                        items.Add(x);
                    }
                }

                var item = new ListViewItem(items.ToArray());
                listView.Items.Add(item);
            }

            toolStripStatusLabel.Text = $"{_reader.MaxDoc} documents";

            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            openButton.Enabled = false;
            closeButton.Enabled = true;
            pathTextBox.Enabled = false;
            refreshButton.Enabled = true;
        }

        private void CloseIndex()
        {
            _reader?.Dispose();
            _directory?.Dispose();

            listView.Items.Clear();
            listView.Columns.Clear();

            openButton.Enabled = true;
            closeButton.Enabled = false;
            pathTextBox.Enabled = true;
            refreshButton.Enabled = false; 
            toolStripStatusLabel.Text = "";
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            CloseIndex();
            OpenIndex();
        }
    }
}