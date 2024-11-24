// <copyright file="Archive.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Vellum.Cli.Commands.Content;

public class Archive
{
}

/*
List<TaxonomyDocument> documents = await loaded.Select(x => x).ToListAsync();
var siteTaxonomyRepository = new SiteDetailsRepository();
SiteDetails siteTaxonomy = await siteTaxonomyRepository.FindAsync(options.SiteTaxonomyDirectoryPath).ConfigureAwait(false);
console.Out.Write(siteTaxonomy.Title + System.Environment.NewLine);

await foreach (TaxonomyDocument doc in loaded)
{
    Console.WriteLine(doc.Path.ToString());
    foreach (ContentFragment contentFragment in doc.ContentFragments)
    {
        if (contentFragment.ContentType == WellKnown.ContentFragments.ContentTypes.BlogMarkdown)
        {
            ContentFragmentTypeFactory<IBlogPost> cff = serviceProvider.GetContent<ContentFragmentTypeFactory<IBlogPost>>(contentFragment.ContentType.AsContentFragmentFactory());
            IBlogPost cf = cff.Create(contentFragment);
            Console.WriteLine(cf.Attachments);
            Console.WriteLine(cf.Author);
            Console.WriteLine(cf.Body);
            Console.WriteLine(cf.Categories);
            Console.WriteLine(cf.ContentType);
            Console.WriteLine(cf.Date);
            Console.WriteLine(cf.Excerpt);
            Console.WriteLine(cf.Faqs);
            Console.WriteLine(cf.HeaderImageUrl);
            Console.WriteLine(cf.IsSeries);
            Console.WriteLine(cf.PartTitle);
            Console.WriteLine(cf.Position);
            Console.WriteLine(cf.Profile);
            Console.WriteLine(cf.PublicationStatus);
            Console.WriteLine(cf.Series);
            Console.WriteLine(cf.Slug);
            Console.WriteLine(cf.Tags);
            Console.WriteLine(cf.Title);
            Console.WriteLine(cf.Url);
            Console.WriteLine(cf.UserName);
        }
    }
}*/

/*foreach (TaxonomyDocument doc in loaded)
{
    foreach (ContentFragment contentFragment in doc.ContentFragments.OrderBy(cf => cf.Date))
    {
        if (contentFragment.ContentType == WellKnown.ContentFragments.ContentTypes.BlogMarkdown)
        {
            ContentFragmentTypeFactory<IBlogPost> contentFragmentTypeFactory = serviceProvider.GetContent<ContentFragmentTypeFactory<IBlogPost>>(contentFragment.ContentType.AsContentFragmentFactory());
            IBlogPost post = contentFragmentTypeFactory.Create(contentFragment);

            IEnumerable<string> categories = post.Category;

            /*if (post.Faqs is { } faqs)
            {
                foreach ((string question, string answer) in faqs)
                {
                    // We're just getting the faq value from the blog, we need
                    // to intercept and convert...
                }
            }#1#

            if (post.Faqs is not null)
            {
                foreach ((string question, string answer) in post.Faqs)
                {
                    // We're just getting the faq value from the blog, we need
                    // to intercept and convert...
                }
            }

            if (post.Author is not null)
            {
                // We're just getting the author value from the blog, we need
                // to intercept and convert...
                IAuthorId authorId = post.Author;
                IAuthor author = authors.First(x => x.UserName == authorId.ToString());
            }

            table.AddRow(
                post.Title,
                post.Author.ToString(),
                post.Date.ToShortDateString(),
                post.PublicationStatus.ToString());
        }
    }
}*/