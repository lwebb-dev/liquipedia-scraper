// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;

List<Player> players = new List<Player>();

HtmlWeb web = new HtmlWeb();
HtmlDocument htmlDoc = web.Load(@"https://liquipedia.net/overwatch/Overwatch_League/Season_5/Regular_Season");

Dictionary<string, HtmlNodeCollection> divisionNodes = new Dictionary<string, HtmlNodeCollection>
{
    { "East", htmlDoc.DocumentNode.SelectNodes("//h3/span[@id=\"East\"]/../following-sibling::div[1]/div/div/div[contains(@class, 'teamcard')]") },
    { "West", htmlDoc.DocumentNode.SelectNodes("//h3/span[@id=\"West\"]/../following-sibling::div/div/div/div[contains(@class, 'teamcard')]") }
};

foreach (KeyValuePair<string, HtmlNodeCollection> divisionNode in divisionNodes)
{
    foreach (HtmlNode teamNode in divisionNode.Value)
    {
        HtmlNodeCollection playerRows = teamNode.ChildNodes[1].FirstChild.FirstChild.ChildNodes;
        playerRows.Remove(playerRows.Count - 1); // Trim Staff Row

        foreach (HtmlNode playerRow in playerRows)
        {
            players.Add(
                new Player
                {
                    Name = playerRow.ChildNodes[1].ChildNodes[2].InnerHtml,
                    Team = teamNode.ChildNodes[0].FirstChild.InnerHtml,
                    Division = divisionNode.Key,
                    Role = playerRow.ChildNodes[0].FirstChild.Attributes["alt"].Value,
                    LiquipediaUri = $"https://liquipedia.net/{playerRow.ChildNodes[1].ChildNodes[2].Attributes["href"].Value}"
                });
        }
    }
}

Console.WriteLine($"Players Found: {players.Count}");
Console.WriteLine($"Teams Found: {players.Select(x => x.Team).Distinct().ToList().Count}");

char delimiter = '|';
string csv = $"Name{delimiter}Team{delimiter}Division{delimiter}Role{delimiter}LiquipediaUri\n";

foreach (Player player in players)
{
    csv += $"{player.Name}{delimiter}{player.Team}{delimiter}{player.Division}{delimiter}{player.Role}{delimiter}{player.LiquipediaUri}\n";
}

File.WriteAllText("output.txt", csv);

public class Player
{
    public string? Name { get; set; }
    public string? Team { get; set; }
    public string? Division { get; set; }
    public string? Role { get; set; }
    public string? LiquipediaUri { get; set; }
}