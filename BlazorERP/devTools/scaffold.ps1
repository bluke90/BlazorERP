# Parameters for Scaffold-DbContext
$connectionString = "server=172.19.8.28;Database=Dev;Trusted_Connection=True;User Id=readonly_user;Password=Read0nly!;Trusted_Connection=false;MultipleActiveResultSets=true;persist security info=true;TrustServerCertificate=True;Integrated Security=false;"
$provider = "Microsoft.EntityFrameworkCore.SqlServer"
$outputDir = "Data/Entities"
$contextDir = "Data/Context"
$contextName = "proContext"

# Running the Scaffold-DbContext command using .NET Core CLI
dotnet ef dbcontext scaffold --no-onconfiguring --use-database-names --verbose $connectionString $provider --output-dir $outputDir --context-dir $contextDir --context $contextName --force --no-build


# Path to database context
$dbContextFile = Join-Path $contextDir "$contextName.cs"

# Custom OnConfiguring method
$customOnConfiguring = @"
    private readonly IConfiguration _configuration;
    
    public proContext(DbContextOptions<proContext> options) : base(options)
    {
    }
    
    public proContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = Program.Configuration;
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();
        
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    } 
"@



# Regex to find the constructor
$constructorRegex = @"
public\s+\w+\s*\(\s*DbContextOptions<\w+>\s*\w*\s*\)\s*:\s*base\(.*?\)\s*\{[^}]*\}
"@

# Get Content
$content = Get-Content $dbContextFile -Raw

# Add "using Microsoft.Extensions.Configuration;" if doesnt exist
if ($content -notmatch "using Microsoft\.Extensions\.Configuration;") {
    Write-Host "Adding 'using Microsoft.Extensions.Configuration;' to the top of the file."
    $content = "using Microsoft.Extensions.Configuration;" + $content
}

if ($content -match $constructorRegex) {
    Write-Host "Constructor found. Replacing with OnConfiguring override method."
    $content = $content -replace $constructorRegex, $customOnConfiguring
    Write-Host "Post Processing Completed..."
} else {
    Write-Host "Constructor not found. No changes made."
}

Set-Content $dbContextFile $content