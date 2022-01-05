// <copyright file="FinancesUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Management.Finances
{
    using SoluiNet.DevTools.Core.Tools.Csv;
    using SoluiNet.DevTools.Core.Tools.File;
    using SoluiNet.DevTools.Management.Finances.Data;
    using SoluiNet.DevTools.Management.Finances.Data.Repositories;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for FinancesUserControl.xaml.
    /// </summary>
    public partial class FinancesUserControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinancesUserControl"/> class.
        /// </summary>
        public FinancesUserControl()
        {
            this.InitializeComponent();

            var repository = new EntryRepository();

            repository.GetAll();

            Entries.ItemsSource = repository.GetAll();
        }

        /// <summary>
        /// Import from Hibiscus CSV file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The event args.</param>
        private void ImportFromHibiscus_Click(object sender, RoutedEventArgs eventArgs)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                RestoreDirectory = true,
                Filter = "Hibiscus CSV File (*.csv)|*.csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };

            var filePath = string.Empty;

            if (fileDialog.ShowDialog() == true)
            {
                filePath = fileDialog.FileName;
            }

            if (!string.IsNullOrEmpty(filePath))
            {
                var csvContents = FileHelper.StreamFromFile(filePath);

                var csvData = CsvHelper.TableFromCsvStream(
                    csvContents,
                    containsHeaders: true,
                    delimiter: ';',
                    lineSeparator: "\n",
                    encoding: Encoding.GetEncoding("ISO-8859-1"));

                var repository = new EntryRepository();

                foreach (DataRow row in csvData.Rows)
                {
                    var entry = new Entry()
                    {
                        Amount = Convert.ToDecimal(row[7]),
                        AdditionalInformation = string.Format(
                            "Purpose 01: {0}\r\nPurpose 02: {1}\r\nPurpose 03: {2}\r\nNote: {3}",
                            row[10],
                            row[11],
                            row[17],
                            row[16]),
                        Date = Convert.ToDateTime(row[9]),
                        ValueDate = Convert.ToDateTime(row[8]),
                        Type = row[18].ToString(),
                        Description = row[10].ToString(),
                    };

                    entry.Account = FindOrCreateAccount(
                            name: row[3].ToString(),
                            bic: row[2].ToString(),
                            iban: row[1].ToString());

                    entry.CounterAccount = FindOrCreateAccount(
                            name: row[6].ToString(),
                            bic: row[5].ToString(),
                            iban: row[4].ToString());

                    entry.Category = FindOrCreateCategory(
                        name: row[15].ToString());

                    repository.Add(entry);
                }
            }
        }

        /// <summary>
        /// Find or create a category if it doesn't exist.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns a <see cref="Category"/> record.</returns>
        private Category FindOrCreateCategory(string name)
        {
            var repository = new CategoryRepository();

            var category = repository.FindByName(name);

            if (category == null)
            {
                category = new Category()
                {
                    Name = name,
                };

                repository.Add(category);
            }

            return category;
        }

        /// <summary>
        /// Find or create an account if it doesn't exist.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="bic">The Business Identifier Code.</param>
        /// <param name="iban">The International Bank Account Number.</param>
        /// <returns>Returns a <see cref="Account"/> record.</returns>
        private Account FindOrCreateAccount(string name, string bic, string iban)
        {
            var repository = new AccountRepository();

            Account account = null;

            if (!string.IsNullOrEmpty(iban))
            {
                account = repository.FindByIban(iban);

                if (account == null)
                {
                    account = new Account()
                    {
                        BIC = bic,
                        IBAN = iban,
                        Name = name,
                    };

                    repository.Add(account);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(name))
                {
                    account = repository.FindByNameAndIban(name, string.Empty);

                    if (account == null)
                    {
                        account = new Account()
                        {
                            BIC = bic,
                            IBAN = iban,
                            Name = name,
                        };

                        repository.Add(account);
                    }
                }
                else
                {
                    account = repository.FindByNameAndIban("Unknown", string.Empty);

                    if (account == null)
                    {
                        account = new Account()
                        {
                            BIC = bic,
                            IBAN = iban,
                            Name = "Unknown",
                        };

                        repository.Add(account);
                    }
                }
            }

            return account;
        }
    }
}
