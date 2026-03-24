# Project Reflection

## About This Document
This document reflects on the development of the semantic search application.
It covers what went well, what was challenging, lessons learned,
and what we would do differently next time.

## What We Built
A working full-stack semantic search application that allows users
to search for short stories using natural language.
The system uses AI-powered embeddings to find stories based on 
meaning and theme, not just exact words.

The application is fully containerised and runs with a single 
Docker command.

## What Went Well

### The Product Works
The most important achievement is that the application is fully 
functional. A user can open the browser, type a natural language 
search query, and receive relevant story results. This was the 
main goal of the project and it was achieved.

### Technical Implementation
- Semantic search using vector embeddings works as intended
- Hybrid search successfully combines semantic and keyword results
- The full stack runs consistently inside Docker with one command
- Backend architecture is clean and well structured with clear 
  separation between controllers, services and repositories
- AI model integration with Ollama works reliably

### Documentation
- Search architecture is documented in detail
- Design thinking and user testing are documented
- API endpoints are clearly defined

## What Was Challenging

### Acting as Product Owner
The most difficult part of this project was the Product Owner role.
Managing a team, defining requirements clearly, and keeping everyone
aligned was harder than expected.

Key challenges included:
- Not having enough team meetings early in the project
- Lack of regular communication led to uncertainty about 
  what to build next
- Team members were not always aware of what others were working on
- GitHub Projects was not used consistently enough to track progress

### Technical Challenges
- Integrating pgvector with Entity Framework required research
  and testing
- Getting Docker services to communicate correctly took time
  to configure
- Understanding how embedding vectors work and how cosine 
  similarity is calculated was a steep learning curve
- Ensuring ingestion worked reliably when Ollama was slow 
  or unavailable

## What We Would Do Differently

### Communication and Team Process
- Hold short regular team meetings more frequently
- Keep all team members informed about progress and blockers
- Discuss implementation decisions as a team before building
- Use GitHub Projects consistently from day one to track all work
- Encourage the whole team to create and update issues regularly

### Product Owner Role
- Define acceptance criteria more clearly before development starts
- Review progress with the team more often
- Ask for help earlier when the PO role felt unclear
- Make sure everyone on the team understands the full picture
  of what we are building and why

### Issue Management
- Break large issues into smaller, more focused tasks
- Each issue should have a single clear purpose
- Write more detailed acceptance criteria so it is clear 
  when an issue is truly done
- Smaller issues are easier to estimate, review and close
- Avoid issues that are too broad — they are hard to track
  and easy to leave half finished

### Technical Decisions
- Agree on a consistent AI model for summaries earlier 
  (we had a mismatch between llama3.2 and gemma3:1b)
- Write tests earlier in the development process
- Set up Docker environment earlier so the full stack 
  could be tested sooner

  ## Performance Considerations

### Current Performance
The application currently holds 10 stories in the database.
At this scale, search performance is fast and acceptable.
No performance optimisation is needed at this stage.

### What Would Change With More Data
As the number of stories grows, the following should be considered:

#### Database
- The current keyword search uses simple text matching
  which works fine for small datasets but would slow down
  significantly with thousands of stories
- The vector similarity search would also slow down
  as more stories are added to the database

#### Backend
- Frequently searched queries could be saved temporarily
  to avoid calling Ollama repeatedly for the same input
- Returning a fixed number of results works for now but
  would need rethinking with a much larger dataset

#### Ollama and Ingestion
- Generating embeddings and summaries is the slowest part
  of loading stories into the database
- Currently stories are processed one by one — story 1 finishes
  completely before story 2 starts
- With many more stories this process could take a very long time

- A message queue system like **RabbitMQ** could improve this
- Instead of processing one story at a time, each story could be
  added to a queue as a separate task
- Multiple workers could then process several stories at the 
  same time, significantly speeding up ingestion
- For example, with 4 workers, 4 stories could be processed
  simultaneously instead of one after another

### Summary
The current implementation is appropriate for the current dataset size.
Performance should be revisited if the number of stories 
grows significantly.

## Future Improvements

These are features and improvements we would like to add 
to the project in the future:

### Search
- Add match strength labels to search results 
  (Strong match, Good match, Partial match)
- Allow users to filter results by genre
- Add a random story feature so users can discover 
  stories without searching

### User Experience
- Make the application fully responsive on mobile devices
- Add bookshelf visual where matching stories light up
  based on search results
- Pin the search bar so it stays visible while scrolling

### Technical
- Introduce RabbitMQ to speed up story ingestion
- Add automated tests for backend services
- Lock down CORS settings before any public deployment
- Fix model mismatch — align summary model to gemma3:1b
  across all configuration files

### Content
- Add more stories to the database
- Add more genres and authors to improve search variety