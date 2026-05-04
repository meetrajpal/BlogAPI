namespace BlogAPI.API.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/posts")]
[ApiVersion("1.0")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
    {
        var result = await _postService.GetAllPostsAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _postService.GetPostByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _postService.GetPostBySlugAsync(slug);
        return Ok(result);
    }

    [HttpGet("author/{authorId:guid}")]
    public async Task<IActionResult> GetByAuthor(Guid authorId, [FromQuery] PaginationFilter filter)
    {
        var result = await _postService.GetPostsByAuthorAsync(authorId, filter);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "CanCreatePost")]
    public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
    {
        var authorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _postService.CreatePostAsync(dto, authorId);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostDto dto)
    {
        var authorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = User.IsInRole("Admin");
        var result = await _postService.UpdatePostAsync(id, dto, authorId, isAdmin);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var authorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = User.IsInRole("Admin");
        var result = await _postService.DeletePostAsync(id, authorId, isAdmin);
        return Ok(result);
    }
}