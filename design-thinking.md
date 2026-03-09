# Design Thinking – Semantic Search Application

## 1. Empathy

### Who is the end user?

End users are readers, students, people that have little time, like parents with small kids who want to discover short stories based on themes, emotions, or situations, not only exact keywords.

### What does the user want?

Users want to quickly find relevant short stories and understand what the story is about before deciding to read the full text.

### What frustrates the user?

Users are frustrated when traditional keyword search does not return relevant results, especially when the story contains the exact search words but not the meaning. Users also find it frustrating when they cannot quickly understand the story content.

### What does the publisher want?

The publisher wants readers to easily discover stories and engage more with their collection. They want to modernize their platform using semantic search and automatic summaries.

### What problems does the publisher have now?

Currently, stories are difficult to discover because traditional keyword search does not capture meaning or themes. This results in low engagement and makes the story collection harder to explore resulting in not return users.

---

## 2. Define the Problem

How might we help users find relevant short stories based on meaning, emotions, and themes, and present them in a clear and engaging way so they can quickly decide what to read?

---

## 3. Ideation

Possible solutions:

* Create a search page where users can enter a natural language query
* Use semantic search to find stories based on meaning, not keywords
* Display search results with:

  * Title
  * Author
  * Genre
  * Year
  * Automatically generated summary
* Allow users to click and read the full story
* Provide a simple and clean user interface

---

## 4. Prototype

The prototype will include three main screens:

### Search Page

User enters a search query and clicks search.

### Results Page

System displays:

* List of relevant stories
* Title
* Author
* Genre
* Year
* Summary

### Story Page

User can read the full story.

The prototype will be created using Figma AI mock website before full implementation.

---

## 5. Testing

Feedback will be collected about:

* Ease of use
* Clarity of results
* Usefulness of summaries

Depending on feedback, the design and functionality will be improved before development continues.
The prototype will be presented to the client.

## Testing

Two informal usability testing sessions were conducted with
team members and stakeholders.

### Session 1 feedback

Users asked whether categories could be selected when searching, suggesting that some expected additional filtering options. Some users were also unsure whether running the same search multiple times would produce the same results, indicating that the behavior of the search system was not fully clear.

The bookshelf feature appeared random to several users, and they questioned whether any factors influence which books appear there. Some users also wondered where the books originate from, suggesting that the purpose of the bookshelf feature could be explained more clearly.

Additionally, a few users asked whether the system contains only short stories and whether stories are always displayed in a single-page format, indicating that the scope of the content may not be immediately obvious.

### Session 2 feedback

Several common themes emerged during the testing sessions.

Users generally liked the clean design, story cards, and the relevance-based search results. The match score and trending searches were frequently mentioned as useful features.

However, the bookshelf feature was not immediately understood by several users. While the idea of a random story was appreciated, the purpose of the bookshelf was not always clear.

Some users also suggested improvements to the story popup layout and visual hierarchy.



## 6. Iteration

Based on the feedback, several improvements were identified:

• Clarify the purpose of the bookshelf feature
• Consider replacing it with a "Surprise Me" button
• Improve hover interactions for story previews
• Highlight bookshelf stories matching the search
• Adjust color palette (sepia suggested instead of red)

## Reflection

The user testing sessions provided valuable insight into how users interpret and interact with the interface. Overall, users responded positively to the clean layout, story cards, and relevance-based search results. Several participants mentioned that the interface was intuitive and visually appealing, and the ability to search for stories based on themes or feelings was seen as a useful feature.

However, the testing also revealed some areas that were not immediately clear to users. In particular, the bookshelf feature caused some confusion. While some users found it fun and interactive, others were unsure how the books were selected or what the feature represented. This indicates that the purpose and logic of the bookshelf component should be communicated more clearly in the interface.

The testing also highlighted the importance of visual clarity and interaction feedback. Some users suggested improvements to the story popup layout and mentioned that certain elements, such as the match score and story titles, could be more visually prominent. Additionally, some users expressed interest in additional filtering options, such as searching for stories suitable for specific age groups.

Overall, the testing confirmed that the core concept of discovering short stories through semantic search works well, but it also emphasized the need for clearer feature explanations and small interface improvements to make the experience more intuitive.