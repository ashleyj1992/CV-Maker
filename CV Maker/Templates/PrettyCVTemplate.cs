using CV_Maker.Interfaces;
using CV_Maker.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CV_Maker.Templates
{
    public class PrettyCVTemplate : ICVTemplate
    {
        private CV _cv { get; set; }
        public CV CV { get => _cv; set => _cv = value; }

        private TextStyle _headerStyle = TextStyle.Default.FontSize(16).Black().FontColor(Colors.Black);
        private TextStyle _subHeaderStyle = TextStyle.Default.FontSize(14).Black().FontColor(Colors.Black);

        public PrettyCVTemplate(CV cv)
        {
            _cv = cv;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(10).SemiBold().FontColor(Colors.Grey.Darken4);
            var nameStyle = TextStyle.Default.FontSize(20).Black().FontColor(Colors.Black);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                    .Text($"{_cv.FirstName} {_cv.LastName}")
                    .Style(nameStyle);

                    foreach (var link in CV.Links)
                    {
                        column.Item()
                        //.Text($"{link.Name}")
                        //.Style(_subHeaderStyle);

                        .Text(x =>
                        {
                            x.Span($"{link.Name}: ").Style(_subHeaderStyle);
                            x.Hyperlink($"{link.URL}", $"{link.URL}").Style(_subHeaderStyle).FontSize(10);
                        });
                    }
                });

                row.RelativeColumn().Column(column =>
                {
                    column.Item()
                    .AlignRight()
                    .PaddingBottom(5)
                    .ContentFromRightToLeft()
                    .Text(_cv.Address.ToString())
                    .Style(titleStyle);

                    column.Item()
                    .AlignRight()
                    .PaddingBottom(5)
                    .Text($"Date of birth: {_cv.DateOfBirth.ToString("dd MMM yyyy")}")
                    .Style(titleStyle);

                    column.Item()
                    .AlignRight()
                    .PaddingBottom(5)
                    .Hyperlink($"mailto:{_cv.EmailAddress}")
                    .Text($"Email address: {_cv.EmailAddress}")
                    .Style(titleStyle);

                    column.Item()
                    .AlignRight()
                    .PaddingBottom(5)
                    .Hyperlink($"tel:{_cv.MobileNumber}")
                    .Text($"Mobile number: {_cv.MobileNumber}")
                    .Style(titleStyle);
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Element(x => ComposeJobs(x, Colors.Blue.Lighten4));
                    column.Spacing(5);
                    column.Item().Element(x => ComposeSkills(x, Colors.Red.Lighten4));
                    column.Spacing(5);
                    column.Item().Element(x => ComposeEducation(x, Colors.Green.Lighten4));
                });
            });
        }

        public void ComposeJobs(IContainer container, string color)
        {
            container.Background(color).Padding(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("Jobs").Style(_headerStyle);
                foreach (var job in CV.Jobs)
                {
                    column.Item().Text(job.CompanyName).Style(_subHeaderStyle);
                    var fromToDate = job.ToDate == null ? $"{job.FromDate.ToString("dd MMM yyyy")} - Current" : $"{job.FromDate.ToString("dd MMM yyyy")} - {job.ToDate?.ToString("dd MMM yyyy")}";
                    column.Item().Text(fromToDate);

                    foreach (var role in job.Roles)
                    {
                        column.Item().Text(role.RoleName).Style(_subHeaderStyle).FontSize(10);
                        column.Item().Text(role.Description).Style(_subHeaderStyle).FontSize(10);
                    }
                }
            });
        }

        public void ComposeSkills(IContainer container, string color)
        {
            container.Background(color).Padding(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("Skills").Style(_headerStyle);
                foreach (var job in CV.Skills)
                {
                    column.Item().Text(job.SkillName).Style(_subHeaderStyle);
                    column.Item().Text(job.Experience.ToString());
                }
            });
        }

        public void ComposeEducation(IContainer container, string color)
        {
            container.Background(color).Padding(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("Education").Style(_headerStyle);
                foreach (var job in CV.Education)
                {
                    column.Item().Text(job.EducationName).Style(_subHeaderStyle);
                    column.Item().Text(job.CourseName.ToString());
                }
            });
        }

        public void Compose(IDocumentContainer container)
        {
            container
            .Page(page =>
            {
                page.Margin(20);

                page.Header().Element(ComposeHeader);

                page.Content().Element(x => ComposeContent(x));
                //page.Content().Element(x => ComposeJobs(x));

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        }
    }
}
