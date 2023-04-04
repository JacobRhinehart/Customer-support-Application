using System;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using Azure;

namespace DLTest
{
    class Program
    {
        static void Main(string [] args) {
        var connectionString = "***connection string from SAS generation***";
        BlobContainerClient container = new BlobContainerClient(connectionString, "****container name here****"); 
        var prefix = "****directory prefix here****";
        Console.WriteLine("------------Starting.----------------");
        Main2(container, prefix, 20).GetAwaiter().GetResult();
        Console.WriteLine("------------Done.----------------");
        }

        private static async Task Main2(BlobContainerClient container, string prefix, int? segmentSize)
        {

            try
            {
                // Call the listing operation and return pages of the specified size.
                var resultSegment = container.GetBlobsByHierarchyAsync(prefix:prefix, delimiter:"/")
                    .AsPages(default, segmentSize);

                // Enumerate the blobs returned for each page.
                await foreach (Page<BlobHierarchyItem> blobPage in resultSegment)
                {
                    // A hierarchical listing may return both virtual directories and blobs.
                    foreach (BlobHierarchyItem blobhierarchyItem in blobPage.Values)
                    {
                        if (blobhierarchyItem.IsPrefix)
                        {
                            // Write out the prefix of the virtual directory.
                            Console.WriteLine("Virtual directory prefix: {0}", blobhierarchyItem.Prefix);

                            // Call recursively with the prefix to traverse the virtual directory.
                            await Main2(container, blobhierarchyItem.Prefix, null);
                        }
                        /**else
                        {
                            // Write out the name of the blob.
                            Console.WriteLine("Blob name: {0}", blobhierarchyItem.Blob.Name);
                        }**/
                    }

                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
