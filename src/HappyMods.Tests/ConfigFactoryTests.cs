using FluentAssertions;
using HappyMods.Core.Config;
using HappyMods.Sort.Config;

namespace HappyMods.Tests;

[Parallelizable(ParallelScope.Fixtures)]
public class ConfigFactoryTests
{
    private ConfigFactory _factory;
    
    [SetUp]
    public void Setup()
    {
        _factory = new($"{Guid.NewGuid()}", new SortConfigDefaultFactory(), new MockUnityConstants());
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(_factory.PersistenceFolderName, true);
    }
    
    [Test]
    public void GetConfig_WillReturnSortConfig_WhenNoFileExists()
    {
        // arrange
        string fileName = SortConfig.FileName;
        
        // act
        var config = _factory.GetConfig<SortConfig>(fileName);

        // assert
        config.Should().NotBeNull();
        File.Exists(Path.Combine(_factory.PersistenceFolderName, $"{fileName}.json")).Should().BeTrue();
    }
    
    [Test]
    public void GetConfig_WillReturnSortItemTabMappingConfig_WhenNoFileExists()
    {
        // arrange
        string fileName = SortItemTabMappingConfig.FileName;
        
        // act
        var config = _factory.GetConfig<SortItemTabMappingConfig>(fileName);

        // assert
        config.Should().NotBeNull();
        File.Exists(Path.Combine(_factory.PersistenceFolderName, $"{fileName}.json")).Should().BeTrue();
    }

    [Test]
    public void GetConfig_WillReturnSortConfig_WhenUpdated()
    {
        // arrange
        string fileName = SortConfig.FileName;
        _factory.GetConfig<SortConfig>(fileName);
        var lastWriteTime = File.GetLastWriteTime(fileName);
        
        // act
        var config = _factory.GetConfig<SortItemTabMappingConfig>(fileName);

        // assert
        config.Should().NotBeNull();
        File.GetLastWriteTime(fileName).Should().Be(lastWriteTime);
    }
    
    [Test]
    public void GetConfig_WillReturnSortItemTabMappingConfig_WhenUpdated()
    {
        // arrange
        string fileName = SortConfig.FileName;
        _factory.GetConfig<SortConfig>(fileName);
        var lastWriteTime = File.GetLastWriteTime(fileName);
        
        // act
        var config = _factory.GetConfig<SortConfig>(fileName);

        // assert
        config.Should().NotBeNull();
        File.GetLastWriteTime(fileName).Should().Be(lastWriteTime);
    }
}