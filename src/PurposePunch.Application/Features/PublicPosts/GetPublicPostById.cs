using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.PublicPosts;

public record GetPublicPostByIdQuery(int Id) : IRequest<PublicPost?>;

public class GetPublicPostByIdHandler : IRequestHandler<GetPublicPostByIdQuery, PublicPost?>
{
    private readonly IPublicPostRepository _repo;

    public GetPublicPostByIdHandler(IPublicPostRepository repo) => _repo = repo;

    public async Task<PublicPost?> Handle(GetPublicPostByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetByIdAsync(request.Id);
    }
}
