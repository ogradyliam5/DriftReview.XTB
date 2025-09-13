using McTools.Xrm.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;

namespace DriftReview.XTB
{
    public partial class DriftReviewControl : PluginControlBase
    {
        private DriftSettings _settings = new DriftSettings();

        public DriftReviewControl()
        {
            InitializeComponent();
            // Hook handlers
            this.Load += DriftReviewControl_Load;
            this.Leave += DriftReviewControl_Leave;

            btnTest.Click += BtnTest_Click;
            btnPreview.Click += BtnPreview_Click;
            btnRun.Click += BtnRun_Click;
            btnExportCsv.Click += BtnExportCsv_Click;
            btnOpenLog.Click += BtnOpenLog_Click;
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);
            LogInfo($"Connected: {detail?.ConnectionName}");
            // Optionally reload per-connection settings
            LoadSettings(perConnection: true);
            ApplySettingsToUi();
        }

        #region Settings
        private string SettingsScopeKey =>
            $"{ConnectionDetail?.ConnectionName ?? "Global"}";

        private void DriftReviewControl_Load(object sender, EventArgs e)
        {
            LoadSettings(perConnection: true);
            ApplySettingsToUi();
            InitGrid();
        }

        private void DriftReviewControl_Leave(object sender, EventArgs e)
        {
            ReadUiToSettings();
            SaveSettings(perConnection: true);
        }

        private void LoadSettings(bool perConnection)
        {
            // Try per-connection first, fall back to global
            if (perConnection && SettingsManager.Instance.TryLoad(GetType(), out DriftSettings s, SettingsScopeKey))
                _settings = s;
            else if (SettingsManager.Instance.TryLoad(GetType(), out DriftSettings sg))
                _settings = sg;
            else
                _settings = new DriftSettings();
        }

        private void SaveSettings(bool perConnection)
        {
            if (perConnection)
                SettingsManager.Instance.Save(GetType(), _settings, SettingsScopeKey);
            else
                SettingsManager.Instance.Save(GetType(), _settings);
        }

        private void ApplySettingsToUi()
        {
            // DateTimePicker uses local time; show local equivalent but keep UTC semantics on read
            dtStart.Value = _settings.StartUtc.ToLocalTime();
            dtEnd.Value = _settings.EndUtc.ToLocalTime();
            txtExclNames.Text = _settings.ExcludedNamesCsv;
            txtExclUsers.Text = _settings.ExcludedUserIdsCsv;
            txtExclApps.Text = _settings.ExcludedAppIdsCsv;
            numPage.Value = Math.Max(numPage.Minimum, Math.Min(numPage.Maximum, _settings.PageSize));
            numMax.Value = Math.Max(numMax.Minimum, Math.Min(numMax.Maximum, _settings.MaxRows));
            splitContainer1.SplitterDistance = _settings.SplitterDistance;
        }

        private void ReadUiToSettings()
        {
            _settings.StartUtc = DateTime.SpecifyKind(dtStart.Value, DateTimeKind.Local).ToUniversalTime();
            _settings.EndUtc = DateTime.SpecifyKind(dtEnd.Value, DateTimeKind.Local).ToUniversalTime();
            _settings.ExcludedNamesCsv = txtExclNames.Text?.Trim() ?? "";
            _settings.ExcludedUserIdsCsv = txtExclUsers.Text?.Trim() ?? "";
            _settings.ExcludedAppIdsCsv = txtExclApps.Text?.Trim() ?? "";
            _settings.PageSize = (int)numPage.Value;
            _settings.MaxRows = (int)numMax.Value;
            _settings.SplitterDistance = splitContainer1.SplitterDistance;
        }
        #endregion

        #region Grid
        private void InitGrid()
        {
            grid.DataSource = null;
            grid.Columns.Clear();

            // Prevent automatic creation of columns from the DataSource.
            grid.AutoGenerateColumns = false;

            grid.Columns.Add("modifiedon", "modifiedon");
            grid.Columns["modifiedon"].DataPropertyName = "modifiedon";

            grid.Columns.Add("modifiedbyname", "modifiedbyname");
            grid.Columns["modifiedbyname"].DataPropertyName = "modifiedbyname";

            grid.Columns.Add("modifiedbytype", "modifiedbytype");
            grid.Columns["modifiedbytype"].DataPropertyName = "modifiedbytype";

            grid.Columns.Add("componentname", "componentname");
            grid.Columns["componentname"].DataPropertyName = "componentname";

            grid.Columns.Add("componenttypename", "componenttypename");
            grid.Columns["componenttypename"].DataPropertyName = "componenttypename";

            grid.Columns.Add("componenttype", "componenttype");
            grid.Columns["componenttype"].DataPropertyName = "componenttype";

            grid.Columns.Add("objectid", "objectid");
            grid.Columns["objectid"].DataPropertyName = "objectid";

            grid.Columns.Add("solutionfriendlyname", "solutionfriendlyname");
            grid.Columns["solutionfriendlyname"].DataPropertyName = "solutionfriendlyname";

            grid.Columns.Add("solutionuniquename", "solutionuniquename");
            grid.Columns["solutionuniquename"].DataPropertyName = "solutionuniquename";

            grid.Columns.Add("resolvedby", "resolvedby");
            grid.Columns["resolvedby"].DataPropertyName = "resolvedby";
        }

        private void BindRows(DataTable dt)
        {
            grid.DataSource = dt;
            lblCounts.Text = $"{dt?.Rows.Count ?? 0} rows";
        }
        #endregion

        #region Buttons
        private void BtnTest_Click(object sender, EventArgs e)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Testing connection...",
                Work = (w, a) =>
                {
                    var who = (WhoAmIResponse)Service.Execute(new WhoAmIRequest());
                    a.Result = who.UserId;
                },
                PostWorkCallBack = a =>
                {
                    if (a.Error != null) { ShowErrorDialog(a.Error); return; }
                    MessageBox.Show($"Connected. UserId: {a.Result}", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            ReadUiToSettings();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Previewing (Top N)…",
                Work = (w, a) =>
                {
                    var ds = new Services.WebApiDataSource(Service);
                    var exclNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    if (!string.IsNullOrWhiteSpace(_settings.ExcludedNamesCsv))
                        foreach (var n in _settings.ExcludedNamesCsv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                            exclNames.Add(n.Trim().ToLowerInvariant());

                    var exclUsers = new HashSet<Guid>();
                    if (!string.IsNullOrWhiteSpace(_settings.ExcludedUserIdsCsv))
                        foreach (var n in _settings.ExcludedUserIdsCsv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                            if (Guid.TryParse(n.Trim(), out var g)) exclUsers.Add(g);

                    var candidates = ds.GetCandidates(_settings.StartUtc, _settings.EndUtc,
                        Math.Min(_settings.PageSize, 500), 50, exclNames, exclUsers); // small preview cap

                    var resolved = ds.ResolveNames(candidates, 200);

                    a.Result = ToDataTable(resolved);
                },
                PostWorkCallBack = a =>
                {
                    if (a.Error != null) { ShowErrorDialog(a.Error); return; }
                    var dt = (System.Data.DataTable)a.Result;
                    BindRows(dt);
                    lblStatus.Text = "Preview complete";
                }
            });
        }


        private void BtnRun_Click(object sender, EventArgs e)
        {
            ReadUiToSettings();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Running drift review…",
                Work = (w, a) =>
                {
                    var ds = new Services.WebApiDataSource(Service);

                    var exclNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    if (!string.IsNullOrWhiteSpace(_settings.ExcludedNamesCsv))
                        foreach (var n in _settings.ExcludedNamesCsv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                            exclNames.Add(n.Trim().ToLowerInvariant());

                    var exclUsers = new HashSet<Guid>();
                    if (!string.IsNullOrWhiteSpace(_settings.ExcludedUserIdsCsv))
                        foreach (var n in _settings.ExcludedUserIdsCsv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                            if (Guid.TryParse(n.Trim(), out var g)) exclUsers.Add(g);

                    var candidates = ds.GetCandidates(_settings.StartUtc, _settings.EndUtc,
                        _settings.PageSize, _settings.MaxRows, exclNames, exclUsers);

                    var resolved = ds.ResolveNames(candidates, 300);

                    a.Result = ToDataTable(resolved);
                },
                PostWorkCallBack = a =>
                {
                    if (a.Error != null) { ShowErrorDialog(a.Error); return; }
                    BindRows((System.Data.DataTable)a.Result);
                    lblStatus.Text = "Run complete";
                },
                IsCancelable = true
            });
        }


        private void BtnExportCsv_Click(object sender, EventArgs e)
        {
            var dt = grid.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.");
                return;
            }

            using (var sfd = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"DriftReview_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv"
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 64))
                    using (var sw = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)))
                    {
                        // header
                        sw.WriteLine(string.Join(",", (dt.Columns.Cast<DataColumn>()).Select(c => Csv(c.ColumnName))));
                        // rows
                        foreach (DataRow r in dt.Rows)
                        {
                            var fields = dt.Columns.Cast<DataColumn>().Select(c => Csv(Convert.ToString(r[c] ?? "")));
                            sw.WriteLine(string.Join(",", fields));
                        }
                        sw.Flush();
                    }

                    lblStatus.Text = $"Exported: {sfd.FileName}";
                }
                catch (Exception ex)
                {
                    ShowErrorDialog(ex);
                }
            }
        }

        private void BtnOpenLog_Click(object sender, EventArgs e)
        {
            try
            {
                var logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logsDir))
                {
                    MessageBox.Show("No Logs folder found in current directory.");
                    return;
                }
                var latest = new DirectoryInfo(logsDir).GetFiles("*.txt")
                    .OrderByDescending(f => f.LastWriteTimeUtc).FirstOrDefault();
                if (latest == null) { MessageBox.Show("No log files found."); return; }

                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = latest.FullName,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex);
            }
        }

        #endregion

        #region Helpers
        private static string Csv(string s)
        {
            if (s == null) return "";
            var mustQuote = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
            s = s.Replace("\"", "\"\"");
            return mustQuote ? $"\"{s}\"" : s;
        }

        private static DataTable MakeTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("modifiedon", typeof(DateTime));
            dt.Columns.Add("modifiedbyname", typeof(string));
            dt.Columns.Add("modifiedbytype", typeof(string));
            dt.Columns.Add("componentname", typeof(string));
            dt.Columns.Add("componenttypename", typeof(string));
            dt.Columns.Add("componenttype", typeof(int));
            dt.Columns.Add("objectid", typeof(Guid));
            dt.Columns.Add("solutionfriendlyname", typeof(string));
            dt.Columns.Add("solutionuniquename", typeof(string));
            dt.Columns.Add("resolvedby", typeof(string));
            return dt;
        }
        private System.Data.DataTable ToDataTable(System.Collections.Generic.IEnumerable<Models.DriftResolved> rows)
        {
            var dt = MakeTable();
            foreach (var r in rows)
            {
                dt.Rows.Add(
                    r.ModifiedOnUtc,
                    r.ModifiedByName,
                    r.ModifiedByType,
                    r.ComponentName,
                    r.ComponentTypeName,
                    r.ComponentType,
                    r.ObjectId,
                    r.SolutionFriendlyName,
                    r.SolutionUniqueName,
                    r.ResolvedBy
                );
            }
            return dt;
        }

        #endregion
    }
}
